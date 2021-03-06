using UnityEngine;

[SelectionBase]
public class GridObject : MonoBehaviour {    

    // Configuration
    public bool Pushable;

    // Runtime
    public Vector3Int Location;

    public delegate void Connect(Side side, GridObject go);
    public event Connect OnConnect;

    public delegate void Disconnect(Side side, GridObject go);
    public event Connect OnDisconnect;

    public GridObject[] Connected;

    public Group Group;

    void Awake() {
        Connected = new GridObject[4];
        Location = transform.position.xyz().RoundToInt();
        transform.position = transform.position.RoundToInt().ToFloat();
        GameController.Instance.SetGridObject(this);
    }

    public void RequestMove(Vector3Int delta) {
    	GameController.Instance.RequestMove(new Move(this, Location, Location + delta));
    }

    public void ConnectSide(Side side) {
        int s = (int)side;
        if (Connected[s] != null) return;
        var go = GameController.Instance.GetGridObject(Location + SideUtil.ToVector(side));
        if (go != null) {
            Connected[s] = go;
            OnConnect?.Invoke(side, Connected[s]);
        }
    }

    public void DisconnectSide(Side side) {
        int s = (int)side;
        if (Connected[s] == null) return;
        var go = GameController.Instance.GetGridObject(Location + SideUtil.ToVector(side));
        if (go != null) {
            Connected[s] = null;
            OnDisconnect?.Invoke(side, Connected[s]);
        }
    }

    public bool IsConnected(GridObject go) {
        for (int i = 0; i < 4; i++) {
            if (Connected[i] == go) return true;
        }
        return false;
    }

    void OnDestroy() {
        GameController.Instance.RemoveGridObject(Location);
    }
}