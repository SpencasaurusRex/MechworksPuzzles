using UnityEngine;
using System.Collections.Generic;

public class GridObject : MonoBehaviour {    

    // Configuration
    public bool Pushable;
	
    // Runtime
    public Vector2Int Location;

    public delegate void Connect(Side side);
    public event Connect OnConnect;

    public delegate void Disconnect(Side side);
    public event Connect OnDisconnect;

    public List<GridObject> Connected = new List<GridObject>();

    public Group Group;

    void Awake() {
        // Group.Add(this);
    }

    void Start() {
        var controller = FindObjectOfType<GameController>();
        Location = transform.position.xy().RoundToInt();
        transform.position = transform.position.RoundToInt().ToFloat();
        controller.SetGridObject(this);

    }

    public void RequestMove(Vector2Int delta) {
    	GameController.Instance.RequestMove(new Move(this, Location, Location + delta));
    }

    public void ConnectSide(Side side) {
        print("Checking for object at " + (Location + SideUtil.ToVector(side)));
        var go = GameController.Instance.GetGridObject(Location + SideUtil.ToVector(side));
        if (go != null && !Connected.Contains(go)) {
            print("Adding connection to " + go.gameObject.name);
            Connected.Add(go);
            OnConnect?.Invoke(side);
        }
    }

    public void DisconnectSide(Side side) {
        var go = GameController.Instance.GetGridObject(Location + SideUtil.ToVector(side));
        if (go != null) {
            Connected.Remove(go);
            OnDisconnect?.Invoke(side);
        }
    }

    void OnDestroy() {
        GameController.Instance.RemoveGridObject(Location);
    }
}