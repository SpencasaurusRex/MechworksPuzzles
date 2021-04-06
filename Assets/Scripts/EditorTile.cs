using UnityEngine;

public class EditorTile : MonoBehaviour {

    public GridType Type;
    public TileInfo Info;

    Vector3Int Location;
    bool firstDrag = true;

    public void OnDrag(Vector2 worldPosition) {
        transform.position = worldPosition.xy().RoundToInt().ToFloat();
    }

    public void EndDrag(Vector2 worldPosition) {
        // Check if that position is valid
        Vector3Int newLocation = worldPosition.xyx().RoundToInt();
        newLocation.z = Util.ToGridLayer(Type);

        if (EditorController.Instance.GetTile(newLocation) != null) {
            if (firstDrag) {
                Destroy(this.gameObject);
            }
            transform.position = Location.ToFloat();
        }
        else {
            if (!firstDrag) EditorController.Instance.RemoveTile(Location);
            Location = newLocation;
            EditorController.Instance.AddTile(Location, this);
        }
        
        firstDrag = false;
    }

    public void Setup(BlockInfo blockInfo) {
        Type = blockInfo.Type;
        Info = Util.ToTileInfo(Type);
        GetComponent<SpriteRenderer>().sprite = blockInfo.Sprite;
    }

    public void RightClick() {
        switch (Type) {

        }
    }
}