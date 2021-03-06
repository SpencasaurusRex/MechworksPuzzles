using UnityEngine;
using UnityEngine.Analytics;

public class Welder : MonoBehaviour
{
    Vector3Int Location;
    GridObject gridObject;

    Welder[] connectedWelders = new Welder[4];

    void Start() {
        GameController.Instance.OnTick += Tick;
        gridObject = GetComponent<GridObject>();
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
            // TODO: Type system?
            if (go.GetComponent<Block>() == null) return;
            for (int i = 0; i < 4; i++) {
                if (connectedWelders[i] == null) continue;
                Vector3Int delta = SideUtil.ToVector((Side)i);
                var otherGo = GameController.Instance.GetGridObject(objectPos + delta);
                if (otherGo != null && otherGo.GetComponent<Block>() != null) {
                    go.ConnectSide((Side)i);
                }
            }
        }
    }
}
