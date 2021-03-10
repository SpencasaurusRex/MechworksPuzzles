using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;

public class GameController : MonoBehaviour
{
    static GameController instance;
    public static GameController Instance => instance;

    public delegate void Tick();
    public event Tick OnTick;
    public event Tick OnTick2;

    // Configuration
    public float TickLength = 1f;
    public bool Running = true;

    public Color[] Colors = { Color.red, Color.green, Color.blue };
 
    // References
    public TextMeshProUGUI TickerText;

    // Runtime
    int tickNumber;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else instance = this;
    }

    void Start() {
        StartCoroutine(TickCoroutine());
    }

    IEnumerator TickCoroutine() {
        while (Running) {
            yield return new WaitForSeconds(TickLength);
            tickNumber++;
            if (TickerText != null)
                TickerText.text = $"Ticks: {tickNumber}";
            OnTick?.Invoke();
            OnTick2?.Invoke();
            HandleRequests();
        }
    }

    HashSet<Vector3Int> validSpaces = new HashSet<Vector3Int>();
    Dictionary<Vector3Int, GridObject> GridObjects = new Dictionary<Vector3Int, GridObject>();

    List<Move> RequestedMoves = new List<Move>();

    public void RequestMove(Move move) {
		RequestedMoves.Add(move);
    }

    public void SetValidSpace(Vector3Int space) {
        validSpaces.Add(space.GroundLayer());
    }

    public void SetGridObject(GridObject obj) {
        GridObjects.Add(obj.Location, obj);
    }

    public void RemoveGridObject(Vector3Int location) {
        GridObjects.Remove(location);
    }
    public GridObject GetGridObject(Vector3Int position) {
        if (GridObjects.ContainsKey(position)) {
            return GridObjects[position];
        }
        return null;
    }

    bool IsValidSpace(Vector3Int location) {
        return validSpaces.Contains(location.GroundLayer());
    }

    List<GridObject> Connected(GridObject block) {
        // TODO: cache this?
        List<GridObject> gos = new List<GridObject>();
        HashSet<GridObject> counted = new HashSet<GridObject>();
        Queue<GridObject> toProcess = new Queue<GridObject>();

        counted.Add(block);
        toProcess.Enqueue(block);

        while (toProcess.Count > 0) {
            var gridObject = toProcess.Dequeue();
            gos.Add(gridObject);
            for (int i = 0; i < 4; i++) {
                var c = gridObject.Connected[i];
                if (c != null && !counted.Contains(c)) {
                    counted.Add(c);
                    toProcess.Enqueue(c);
                }
            }
        }
        return gos;
    }

    void HandleRequests() {
        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

        // 1. Propagate moves
        List<Move> propagatedMoves = new List<Move>();
        foreach (var move in RequestedMoves) {
            propagatedMoves.Add(move);     
            var go = GetGridObject(move.To);
            // Pushes
            if (go != null) {
                var wholeBlock = Connected(go);
                foreach (var block in wholeBlock) {
                    var pushMove = new Move(block, block.Location, block.Location + move.Delta);
                    pushMove.Dependents.Add(move);
                    move.Dependents.Add(pushMove);
                    propagatedMoves.Add(pushMove);
                }
            }
            // Connections
            for (int i = 0; i < 4; i++) {
                go = move.Object.Connected[i];
                if (go == null) continue;
                var wholeBlock = Connected(go);
                foreach (var block in wholeBlock) {
                    var dragMove = new Move(block, block.Location, block.Location + move.Delta);
                    dragMove.Dependents.Add(move);
                    move.Dependents.Add(dragMove);
                    propagatedMoves.Add(dragMove);
                }
            }
        }


        // 2. Move duplication checks
        Dictionary<Vector3Int, Move> moveStarts = new Dictionary<Vector3Int, Move>();
        Dictionary<Vector3Int, Move> moveEnds = new Dictionary<Vector3Int, Move>();
        List<Move> distinctMoves = new List<Move>();
        foreach (var move in propagatedMoves) {
            if (moveStarts.ContainsKey(move.From)) {
                var existingMove = moveStarts[move.From];
                if (existingMove.To == move.To) {
                    // Same destination, no problem
                }
                else {
                    move.Block();
                    existingMove.Block();
                }
            }
            else {
                moveStarts.Add(move.From, move);
                distinctMoves.Add(move);
            }

            if (moveEnds.ContainsKey(move.To)) {
                var existingMove = moveEnds[move.To];
                if (existingMove.From == move.From) {
                    // Same move, just deduplicate
                }
                else {
                    move.Block();
                    existingMove.Block();
                }
            }
            else {
                moveEnds.Add(move.To, move);
            }
        }


        // 3. Setup dependency chain
        foreach (var move in distinctMoves) {
            var go = GetGridObject(move.To);
            if (go == null) continue;
            // Something is in our way, check if it is moving
            if (moveStarts.ContainsKey(move.To)) {
                var otherMove = moveStarts[move.To];
                if (otherMove.Blocked) {
                    move.Block();
                }
                else if (otherMove.Delta == move.Delta) {
                    // Setup dependency
                    if (!otherMove.Dependents.Contains(move)) {
                        otherMove.Dependents.Add(move);
                    }
                }
                else {
                    // Moving in a different direction
                    move.Block();
                }
            }
            else {
                move.Block();
            }
            // Add dependencies to connected blocks
            for (int i = 0; i < 4; i++) {
                var connection = move.Object.Connected[i];
                if (connection == null) continue;
                var connectionLocation = move.From + SideUtil.ToVector(i);
                if (!moveStarts.ContainsKey(connectionLocation)) {

                }
            }
        }


        // 4. Evaluate the moves
        foreach (var move in distinctMoves) {
            if (move.Blocked) continue;
            if (!IsValidSpace(move.To)) {
                move.Block();
            }
        }

        
        // 5. Perform the moves
        foreach (var move in distinctMoves) {
            if (move.Blocked) continue;
            GridObjects.Remove(move.From);
        }
        foreach (var move in distinctMoves) {
            if (move.Blocked) continue;
            GridObjects.Add(move.To, move.Object);
            move.Object.Location = move.To;
            StartCoroutine(LerpMove(move));
        }


        RequestedMoves.Clear();

        sw.Stop();
    }

    IEnumerator LerpMove(Move move) {
        float t = 0;
        while (t < 1 && move.Object != null)
        {
            move.Object.transform.position = Vector3.Lerp(move.From, move.To, t);
            t += Time.deltaTime / TickLength;
            yield return new WaitForEndOfFrame();
        }

        if (move.Object != null) {
            move.Object.transform.position = move.To.ToFloat();
        }
    }

    void OnDrawGizmos() {
        //foreach (var pos in validSpaces)
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawCube(pos.ToFloat(), Vector3.one * 0.6f);
        //}

        foreach (var pos in GridObjects.Keys)
        {
            if (pos.z == -1) {
                continue;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pos.ToFloat(), Vector3.one * 0.5f);
        }
    }
}