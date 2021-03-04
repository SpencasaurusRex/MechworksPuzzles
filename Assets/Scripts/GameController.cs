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

    // Configuration
    public float TickLength = 1f;
    public bool Running = true;

    public Color Blue = Color.blue;
    public Color Red = Color.red;
    public Color Green = Color.green;

    // References
    public TextMeshProUGUI TickerText;

    // Runtime
    int tickNumber;

    void Start() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else instance = this;
        StartCoroutine(TickCoroutine());
    }

    IEnumerator TickCoroutine() {
        while (Running) {
            yield return new WaitForSeconds(TickLength);
            tickNumber++;
            TickerText.text = $"Ticks: {tickNumber}";
            OnTick?.Invoke();
            DetermineGroups();
            HandleRequests();
        }
    }

    HashSet<Vector2Int> validSpaces = new HashSet<Vector2Int>();
    Dictionary<Vector2Int, GridObject> GridObjects = new Dictionary<Vector2Int, GridObject>();
    
    List<Move> RequestedMoves = new List<Move>();

    public void RequestMove(Move move) {
		RequestedMoves.Add(move);
    }

    public void SetValidSpace(Vector2Int space) {
        validSpaces.Add(space);
    }

    public void SetGridObject(GridObject obj) {
        GridObjects.Add(obj.Location, obj);
    }

    public void RemoveGridObject(Vector2Int location) {
        GridObjects.Remove(location);
    }
    public GridObject GetGridObject(Vector2Int position) {
        if (GridObjects.ContainsKey(position)) {
            return GridObjects[position];
        }
        return null;
    }

    void DetermineGroups() {
        // Based on flood-fill algorithm
        // TODO
    }

    void HandleRequests() {
        foreach (var request in RequestedMoves) {
            print(request.From + " " + request.To + ": " + request.Object.Connected.Count);
        }

        List<Move> validMoves = new List<Move>();
        for (int i = 0; i < RequestedMoves.Count; i++)
        {
            var request = RequestedMoves[i];
            var offset = request.To - request.From;
			if (!validSpaces.Contains(request.To)) {
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

            Move pushMove = null;

            // TODO: Allow move chaining
            if (GridObjects.ContainsKey(request.To)) {
                var go = GridObjects[request.To];
                if (!request.Object.Connected.Contains(go)) {
                    if (!go.Pushable) {
                        continue;
                    }
                    var pushableTarget = go.Location + offset;
                    // Make sure pushable can move
                    if (!validSpaces.Contains(pushableTarget)) {
                        continue;
                    }
                    if (GridObjects.ContainsKey(pushableTarget)) {
                        continue;
                    }
                    pushMove = new Move(go, request.To, pushableTarget);
                }
            }

            // Validate connected
            bool connectedValid = true;
            foreach (var connected in request.Object.Connected) {
                var connectedDestination = connected.Location + offset;
                if (!validSpaces.Contains(connectedDestination)) {
                    connectedValid = false;
                    break;
                }
                if (GridObjects.ContainsKey(connectedDestination)) {
                    var blocker = GridObjects[connectedDestination];
                    if (blocker != request.Object) {
                        connectedValid = false;
                        break;
                    }
                }
                validMoves.Add(new Move(connected, connected.Location, connectedDestination));
            }
            if (!connectedValid) continue;

            print(request.From + " " + request.To);
            validMoves.Add(request);
            if (pushMove != null) {
                print(pushMove.From + " " + pushMove.To);
                validMoves.Add(pushMove);
            }
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
            move.Object.transform.position = Vector2.Lerp(move.From, move.To, t);
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
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pos.ToFloat(), Vector3.one * 0.5f);
        }
    }
}