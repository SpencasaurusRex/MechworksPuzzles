using System.IO;
using UnityEngine;

public class MapRunner : MonoBehaviour {
    public string MapPath;

    void Start() {
        var text = Resources.Load<TextAsset>(MapPath);

        LoadMap(new MemoryStream(text.bytes));
    }
    
    public void LoadMap(Stream stream) {
        MapInfo info = MapReader.ReadMap(stream);

        var empty = GameData.Instance.EmptyGameObjectPrefab;

        var world = Instantiate(empty);
        world.name = "World";

        var ground = Instantiate(empty, Vector3.zero, Quaternion.identity, world.transform);
        ground.name = "Ground";

        var obj = Instantiate(empty, Vector3.zero, Quaternion.identity, world.transform);
        obj.name = "Object";

        for (int i = 0; i < info.NumberOfTiles; i++) {
            var tileData = info.Tiles[i];
            var (x, y, layer) = tileData.GetPosition();
            Vector3Int position = new Vector3Int(x, y, layer);

            var prefab = GameData.Instance.TilePrefabs[tileData.GetGridType()];
            Transform t = position.z == GridLayer.Ground ? ground.transform : obj.transform;

            GameObject go = Instantiate(prefab, position, Quaternion.identity, t);
            
            switch (tileData.GetGridType()) {
                case GridType.Target:
                    go.GetComponent<Target>().AssignData(tileData);
                    break;
                case GridType.Robot:
                    go.GetComponent<Robot>().AssignData(tileData);
                    break;
                case GridType.Block:
                    go.GetComponent<Block>().AssignData(tileData);
                    break;
                case GridType.Spawner:
                    go.GetComponent<Spawner>().AssignData(tileData);
                    break;
            }
        }
    }
}