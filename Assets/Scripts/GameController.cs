using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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
            TickerText.text = $"Ticks: {tickNumber}";
            OnTick?.Invoke();
            OnTick2?.Invoke();
            DetermineGroups();
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

    void DetermineGroups() {
        // Based on flood-fill algorithm
        // TODO
    }

    bool IsValidSpace(Vector3Int location) {
        return validSpaces.Contains(location.GroundLayer());
    }

    bool MoveBlock(GridObject block, Vector3Int delta, GridObject pusher, List<Move> validMoves) {
        // Accumulate connected blocks
        HashSet<GridObject> counted = new HashSet<GridObject>();
        List<GridObject> gos = new List<GridObject>();
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

        foreach (var go in gos) {
            var targetLocation = go.Location + delta;
            var targetObject = GetGridObject(targetLocation);
            if (targetObject != null) {
                // TODO: Allow move chaining
                if (!counted.Contains(targetObject) && targetObject != pusher) {
                    // There is something in our way
                    return false;
                }
            }
            if (!IsValidSpace(targetLocation)) {
                return false;
            }
        }

        foreach (var go in gos) {
            var targetLocation = go.Location + delta;
            validMoves.Add(new Move(go, go.Location, targetLocation));
        }
        return true;
    }

    void HandleRequests() {
        List<Move> validMoves = new List<Move>();
        for (int i = 0; i < RequestedMoves.Count; i++)
        {
            var request = RequestedMoves[i];
            var offset = request.To - request.From;
			if (!IsValidSpace(request.To)) {
                continue;
            }
            
            bool duplicateDestination = false;
            for (int j = 0; j < RequestedMoves.Count; j++) {
                if (j == i) continue;
                if (request.To == RequestedMoves[j].To) {
                    duplicateDestination = true;
                    break;
                }
            }
            if (duplicateDestination) continue;

            List<Move> externalMoves = new List<Move>();

            var delta = request.To - request.From;

            // Check pushes
            if (GridObjects.ContainsKey(request.To)) {
                var go = GridObjects[request.To];
                if (!request.Object.IsConnected(go)) {
                    if (!go.Pushable) {
                        continue;
                    }
                    if (!MoveBlock(go, delta, request.Object, externalMoves)) {
                        continue;
                    }
                }
            }

            // Check connected
            var connectedValid = true;
            foreach (var connected in request.Object.Connected) {
                if (connected == null) {
                    continue;
                }
                if (!MoveBlock(connected, delta, request.Object, externalMoves)) {
                    connectedValid = false;
                }
            }
            if (!connectedValid) continue;

            validMoves.Add(request);
            validMoves.AddRange(externalMoves);
		}

        foreach (var move in validMoves) {
            GridObjects.Remove(move.From);
        }
        foreach (var move in validMoves) {
            GridObjects.Add(move.To, move.Object);
            move.Object.Location = move.To;
            StartCoroutine(LerpMove(move));
        }

        RequestedMoves.Clear();
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