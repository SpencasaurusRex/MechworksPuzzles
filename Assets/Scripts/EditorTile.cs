using UnityEngine;

public class EditorTile : MonoBehaviour {

    public GridType Type;
    public TileData Data;
    public Vector3Int Location;
    public SpriteRenderer sr;

    public void Setup(TileSelectInfo tileSelectInfo, Vector3Int location) {
        Type = tileSelectInfo.Type;
        Data = Util.ToTileInfo(Type);
        Location = location;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = tileSelectInfo.Sprite;
        sr.sortingOrder = Location.z;
    }

    public void Setup(TileData tileInfo, Vector3Int location) {
        Type = tileInfo.GetGridType();
        Data = tileInfo;
        Location = location;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = GameData.Instance.GridSprites[Type];
        sr.sortingOrder = Location.z;

        switch (Type) {
            case GridType.Block:
            case GridType.Spawner:
            case GridType.Target:
                var colorData = tileInfo as ColorTileInfo;
                sr.color = GameData.Instance.Colors[(int)colorData.Color];
                break;
        }
    }

    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetVisible(bool visible) {
        sr.enabled = visible;
    }
}