using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelectPanel : MonoBehaviour
{
    // Configuration
    public TileSelectInfo[] TileInfos;
    public TileSelect TileSelectPrefab;
    public Transform TilesParent;
    public EditorTile EditorTilePrefab;

    // Runtime
    TileSelect selectedTile;
    Vector3 LastMouseWorldPosition;


    void Start() {
        var groundGroup = transform.Find("GroundLayerGroup");
        var objectGroup = transform.Find("ObjectLayerGroup");

        foreach (var info in TileInfos) {
            Transform group = info.Layer == GridLayer.Ground ? groundGroup : objectGroup;

            var tileSelect = Instantiate(TileSelectPrefab, Vector3.zero, Quaternion.identity, group);
            tileSelect.Setup(info, this);
            if (selectedTile == null) SelectTile(tileSelect);
        }

        LastMouseWorldPosition = Util.MouseWorldPosition(Camera.main);
    }

    void Update() {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
            var pos = Util.MouseTilePosition(Camera.main);
            pos.z = selectedTile.info.Layer;
            var existingTile = EditorController.Instance.GetTile(pos);
            if (existingTile) {
                EditorController.Instance.RemoveTile(pos);
                Destroy(existingTile.gameObject);
            }
            if (selectedTile.info.Type != GridType.None) {
                var newTile = Instantiate(EditorTilePrefab, pos, Quaternion.identity, TilesParent);
                newTile.Setup(selectedTile.info, pos);
                EditorController.Instance.AddTile(pos, newTile);
                if (selectedTile.info.Layer == GridLayer.Ground) EditorController.Instance.SetGroundLayerVisible(true);
                if (selectedTile.info.Layer == GridLayer.Object) EditorController.Instance.SetObjectLayerVisible(true);
            }
        }

        if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject()) {
            var pos = Util.MouseTilePosition(Camera.main).WithZ(selectedTile.info.Layer);
            var tile = EditorController.Instance.GetTile(pos);
            if (tile != null) {
                EditorController.Instance.RemoveTile(pos);
                Destroy(tile.gameObject);
            }
        }
    }

    public void SelectTile(TileSelect tile) {
        if (selectedTile != null) {
            selectedTile.SetSelected(false);
        }
        selectedTile = tile;
        selectedTile.SetSelected(true); 
        if (selectedTile.info.Layer == GridLayer.Ground) {
            EditorController.Instance.SetGroundLayerVisible(true);
        }
        else if (selectedTile.info.Layer == GridLayer.Object) {
            EditorController.Instance.SetObjectLayerVisible(true);
        }
    }
}