using UnityEngine;

public class Spawner : MonoBehaviour {
    // Configuration
    public ColorTileInfo Data = new ColorTileInfo(GridType.Spawner, ObjectColor.None);

    public GameObject[] BlockPrefabs;

    // Runtime
    GridObject gridObject;

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Spawner;
    }

    void Start() {
        GameController.Instance.OnTick += Tick;
        GameController.Instance.SetValidSpace(gridObject.Location);

        Create();
    }

    public void AssignData(TileData data) {
        Data = data as ColorTileInfo;
        var sr = GetComponent<SpriteRenderer>();
        sr.color = GameData.Instance.Colors[(int)Data.Color];
    }

    void Tick() {
        Create();
    }

    void Create() {
        var objPosition = gridObject.Location.ObjectLayer();
        if (GameController.Instance.GetGridObject(objPosition) == null) {
            GameObject prefab = BlockPrefabs[(int)Data.Color];
            Instantiate(prefab, objPosition.ToFloat(), Quaternion.identity);
        }
    }
}
