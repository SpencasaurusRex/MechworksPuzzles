using UnityEngine;
using UnityEngine.EventSystems;

public class PropertiesPanel : MonoBehaviour {
    
    public GameObject HighlightRectSelected;
    public EditorTile SelectedTile;

    NonePanel nonePanel;
    ColorPanel colorPanel;

    void Start() {
        nonePanel = transform.Find("NonePanel").GetComponent<NonePanel>();
        colorPanel = transform.Find("ColorPanel").GetComponent<ColorPanel>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            var location = Util.MouseTilePosition(Camera.main);
            location.z = GridLayer.Object;
            if (SelectedTile && SelectedTile.Location.x == location.x && SelectedTile.Location.y == location.y) {
                // We clicked the same column
                location.z = SelectedTile.Location.z == GridLayer.Ground ? GridLayer.Object : GridLayer.Ground;
                var tile = EditorController.Instance.GetTile(location);
                if (tile != null) SelectTile(tile);
            }
            else {
                var objectTile = EditorController.Instance.GetTile(location);
                var groundTile = EditorController.Instance.GetTile(location.WithZ(GridLayer.Ground));

                if (objectTile) {
                    SelectTile(objectTile);
                }
                else if (groundTile) {
                    SelectTile(groundTile);
                }
                else SelectTile(null);
            }
        }

        if (SelectedTile != null) {
            HighlightRectSelected.transform.position = SelectedTile.Location;
        }
    }

    public void SelectTile(EditorTile tile) {
        HighlightRectSelected.SetActive(tile != null);
        SelectedTile = tile;
        
        nonePanel.gameObject.SetActive(false);
        colorPanel.gameObject.SetActive(false);

        TilePropertiesPanel panel = null;

        if (SelectedTile != null) {
            switch (SelectedTile.Type) {
                case GridType.Block:
                case GridType.Spawner:
                case GridType.Target:
                    colorPanel.gameObject.SetActive(true);
                    panel = colorPanel;
                    break;
                default: 
                    nonePanel.gameObject.SetActive(true);
                    panel = nonePanel;
                    break;
            }
        }

        if (panel != null) {
            panel.Assign(tile);
        }
    }

    public void ShowPanel() {
        if (SelectedTile != null) {
            HighlightRectSelected.SetActive(true);
        }
    }

    public void HidePanel() {
        HighlightRectSelected.SetActive(false);
        gameObject.SetActive(false);
    }
}
