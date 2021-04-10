using UnityEngine;

public class EditorTile : MonoBehaviour {

    public GridType Type;
    public TileData Info;
    public Vector3Int Location;
    public SpriteRenderer sr;

    public void Setup(TileSelectInfo tileSelectInfo, Vector3Int location) {
        Type = tileSelectInfo.Type;
        Info = Util.ToTileInfo(Type);
        Location = location;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = tileSelectInfo.Sprite;
        sr.sortingOrder = Location.z;
    }

    public void Setup(TileData tileInfo, Vector3Int location) {
        Type = tileInfo.GetGridType();
        Info = tileInfo;
        Location = location;
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = GameData.Instance.GridSprites[Type];
        sr.sortingOrder = Location.z;
    }

    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetVisible(bool visible) {
        sr.enabled = visible;
    }

    public void RightClick() {
        
    }
}