using UnityEngine;
using UnityEngine.Analytics;

public class Welder : MonoBehaviour
{
    Vector3Int Location;
    GridObject gridObject;

    Welder[] connectedWelders = new Welder[4];

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Welder;
    }

    void Start() {
        GameController.Instance.OnTick += Tick;
        GameController.Instance.SetValidSpace(gridObject.Location);

        // Find connected welders
        for (int i = 0; i < 4; i++) {
            var go = GameController.Instance.GetGridObject(gridObject.Location + SideUtil.ToVector((Side)i));
            if (go == null) continue;
            connectedWelders[i] = go.GetComponent<Welder>();
        }
    }

    void Tick() {
        var objectPos = gridObject.Location.ObjectLayer();
        var go = GameController.Instance.GetGridObject(objectPos);
        
        if (go != null) {
            if (go.Type != GridType.Block) return;
            for (int i = 0; i < 4; i++) {
                if (connectedWelders[i] == null) continue;
                Vector3Int delta = SideUtil.ToVector((Side)i);
                var otherGo = GameController.Instance.GetGridObject(objectPos + delta);
                if (otherGo != null && otherGo.Type == GridType.Block) {
                    go.ConnectSide((Side)i);
                }
            }
        }
    }
}
