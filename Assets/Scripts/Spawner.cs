using UnityEngine;

public class Spawner : MonoBehaviour {
    // Configuration
    public ObjectColor Color;

    public GameObject[] BlockPrefabs;

    // Runtime
    GridObject gridObject;

    void Start() {
        GameController.Instance.OnTick += Tick;
        gridObject = GetComponent<GridObject>();
        GameController.Instance.SetValidSpace(gridObject.Location);

        Create();

        var sr = GetComponent<SpriteRenderer>();
        sr.color = GameController.Instance.Colors[(int)Color];
    }

    void Tick() {
        Create();
    }

    void Create() {
        var objPosition = gridObject.Location.ObjectLayer();
        if (GameController.Instance.GetGridObject(objPosition) == null) {
            GameObject prefab = BlockPrefabs[(int)Color];
            Instantiate(prefab, objPosition.ToFloat(), Quaternion.identity);
        }
    }
}
