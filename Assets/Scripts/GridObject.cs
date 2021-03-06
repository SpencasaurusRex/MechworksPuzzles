using UnityEngine;

public class GridObject : MonoBehaviour {    

    // Configuration
    public bool Movable;

    // Runtime
    public Vector3Int Location;

    public delegate void Connect(Side side, GridObject go);
    public event Connect OnConnect;

    public delegate void Disconnect(Side side, GridObject go);
    public event Connect OnDisconnect;

    public GridType Type;
    public GridObject[] Connected;

    void Awake() {
        Connected = new GridObject[4];
        Location = transform.position.RoundToInt();
        transform.position = transform.position.RoundToInt().ToFloat();
        GameController.Instance.SetGridObject(this);
    }

    void Update() {
        
    }

    public void RequestMove(Vector3Int delta) {
    	GameController.Instance.RequestMove(new Move(this, Location, Location + delta));
    }

    public void ConnectSide(Side side) {
        int s = (int)side;
        if (Connected[s] != null) return;
        var go = GameController.Instance.GetGridObject(Location + Util.ToDelta(side));
        
        if (go != null) {
            Connected[s] = go;
            OnConnect?.Invoke(side, Connected[s]);
            int os = (s + 2) % 4;
            if (go.Connected[os] == null) {
                go.ConnectSide((Side)os);
            }
        }
    }

    public void DisconnectSide(Side side) {
        int s = (int)side;
        if (Connected[s] == null) return;
        var oldConnection = Connected[s];
        Connected[s] = null;
        OnDisconnect?.Invoke(side, Connected[s]);
        int os = (s + 2) % 4;
        oldConnection.DisconnectSide((Side)os);
    }

    void OnDestroy() {
        GameController.Instance.RemoveGridObject(Location);
        for (int i = 0; i < 4; i++) {
            if (Connected[i] != null) {
                DisconnectSide((Side)i);
            }
        }
    }
}