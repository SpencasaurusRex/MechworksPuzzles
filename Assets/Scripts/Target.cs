using UnityEngine;

public class Target : MonoBehaviour {
    // Configuration
    public ColorTileInfo Data;

    // Runtime
    GridObject gridObject;
    int currentCount;
    Target[] neighborTargets;

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Target;
    }

    void Start() {
        GameController.Instance.OnTick += Tick;
        GameController.Instance.OnTick2 += Tick2;

        GameController.Instance.SetValidSpace(gridObject.Location);

        var sr = GetComponent<SpriteRenderer>();
        sr.color = GameController.Instance.Colors[(int)Data.Color];

        neighborTargets = new Target[4];
        for (int i = 0; i < 4; i++) {
            var neighborPos = gridObject.Location + SideUtil.ToVector((Side)i);
            var neighborGo = GameController.Instance.GetGridObject(neighborPos.GroundLayer());
            if (neighborGo != null) {
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
            if (box != null && box.Color == Data.Color) {
                Satisfied = true;
            }
        }
    }

    void Tick2() {
        if (!Satisfied) return;
        for (int i = 0; i < 4; i++) {
            if (neighborTargets[i] != null && !neighborTargets[i].Satisfied) {
                return;
            }
        }
        var go = GameController.Instance.GetGridObject(gridObject.Location.ObjectLayer());
        Destroy(go.gameObject);
        currentCount++;
    }
}