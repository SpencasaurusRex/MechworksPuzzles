using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelectPanel : MonoBehaviour
{
    // Configuration
    public TileSelectInfo[] TileInfos;
    public TileSelect TileSelectPrefab;
    public Transform TilesParent;
    public EditorTile EditorTilePrefab;
    public Transform HighlightRect;

    // Runtime
    TileSelect selectedTile;
    Vector3 MouseWorldPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 LastMouseWorldPosition;
    Vector3Int MouseTilePosition => MouseWorldPosition.RoundToInt();

    void Start() {
        var groundGroup = transform.Find("GroundLayerGroup");
        var objectGroup = transform.Find("ObjectLayerGroup");

        foreach (var info in TileInfos) {
            Transform group = info.Layer == GridLayer.Ground ? groundGroup : objectGroup;

            var tileSelect = Instantiate(TileSelectPrefab, Vector3.zero, Quaternion.identity, group);
            tileSelect.Setup(info, this);
            if (selectedTile == null) SelectTile(tileSelect);
        }

        LastMouseWorldPosition = MouseWorldPosition;
    }

    void Update() {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
            var pos = MouseTilePosition;
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
            }
        }

        if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject()) {
            var pos = MouseTilePosition.WithZ(selectedTile.info.Layer);
            var tile = EditorController.Instance.GetTile(pos);
            if (tile != null) {
                EditorController.Instance.RemoveTile(pos);
                Destroy(tile.gameObject);
            }
        }

        if (Input.GetMouseButton(2)) {
            Camera.main.transform.position += (LastMouseWorldPosition - MouseWorldPosition);
        }

        //if (!EventSystem.current.IsPointerOverGameObject()) 
        {
            HighlightRect.position = Vector3.Lerp(HighlightRect.position, MouseTilePosition.WithZ(5), 0.4f);
        }

        LastMouseWorldPosition = MouseWorldPosition;
    }

    public void SelectTile(TileSelect tile) {
        if (selectedTile != null) {
            selectedTile.SetSelected(false);
        }
        selectedTile = tile;
        selectedTile.SetSelected(true);
    }
}