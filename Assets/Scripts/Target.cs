using UnityEngine;

public class Target : MonoBehaviour
{
    // Configuration
    public int GoalCount;
    public ObjectColor GoalColor;

    // If neighboring targets are synced
    public bool[] SyncedNeighbors = new bool[4];

    // Runtime
    GridObject gridObject;
    int currentCount;
    Target[] neighborTargets = new Target[4];

    void Start() {
        GameController.Instance.OnTick += Tick;
        GameController.Instance.OnTick2 += Tick2;

        gridObject = GetComponent<GridObject>();
        GameController.Instance.SetValidSpace(gridObject.Location);

        var sr = GetComponent<SpriteRenderer>();
        sr.color = GameController.Instance.Colors[(int)GoalColor];

        for (int i = 0; i < 4; i++) {
            if (SyncedNeighbors[i]) {
                var neighborPos = gridObject.Location + SideUtil.ToVector((Side)i);
                var neighborGo = GameController.Instance.GetGridObject(neighborPos.GroundLayer());
                neighborTargets[i] = neighborGo.GetComponent<Target>();
            }
        }
    }

    public bool Satisfied;
    
    void Tick() {
        Satisfied = false;
        var go = GameController.Instance.GetGridObject(gridObject.Location.ObjectLayer());
        if (go != null) {
            var box = go.GetComponent<Block>();
            if (box != null && box.Color == GoalColor) {
                Satisfied = true;
            }
        }
    }

    void Tick2() {
        for (int i = 0; i < 4; i++) {
            if (SyncedNeighbors[i] && !neighborTargets[i].Satisfied) {
                return;
            }
        }
        var go = GameController.Instance.GetGridObject(gridObject.Location.ObjectLayer());
        Destroy(go.gameObject);
        currentCount++;
    }
}