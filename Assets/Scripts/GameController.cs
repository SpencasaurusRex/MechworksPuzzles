using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameController : MonoBehaviour
{
    static GameController instance;
    public static GameController Instance => instance;

    public delegate void Tick();
    public event Tick OnTick;

    // Configuration
    public float TickLength = 1f;
    public bool Running = true;

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
            OnTick?.Invoke();
            HandleRequests();
        }
    }

    HashSet<Vector2Int> validSpaces = new HashSet<Vector2Int>();
    Dictionary<Vector2Int, GridObject> GridObjects = new Dictionary<Vector2Int, GridObject>();
    
    List<Move> RequestedMoves = new List<Move>();
    // HashSet<Vector2Int> Pushes = new HashSet<Vector2Int>();

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

    void HandleRequests() {
		List<Move> validMoves = new List<Move>();
        for (int i = 0; i < RequestedMoves.Count; i++)
        {
            var request = RequestedMoves[i];
			if (!validSpaces.Contains(request.To)) {
                continue;
            }
            // TODO: Allow move chaining
            // TODO: Connected aren't pushables
            if (GridObjects.ContainsKey(request.To)) {
                var go = GridObjects[request.To];
                if (!go.Pushable) {
                    continue;
                }
                var pushableTarget = go.Location + request.To - request.From;
                // Make sure pushable can move
                if (!validSpaces.Contains(pushableTarget)) {
                    continue;
                }
                if (GridObjects.ContainsKey(pushableTarget)) {
                    continue;
                }
                validMoves.Add(new Move(go, request.To, pushableTarget));
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

            validMoves.Add(request);
		}

        foreach (var move in validMoves) {
            GridObjects.Remove(move.From);
        }
        foreach (var move in validMoves) {
            GridObjects.Add(move.To, move.Object);
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
    }

    void OnDrawGizmos() {
        //foreach (var move in RequestedMoves) {
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawCube(move.From.ToFloat(), Vector3.one * 0.3f);
        //    Gizmos.DrawLine(move.From.ToFloat(), move.To.ToFloat());
        //}
        foreach (var pos in GridObjects.Keys)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pos.ToFloat(), Vector3.one * 0.5f);
        }
    }
}