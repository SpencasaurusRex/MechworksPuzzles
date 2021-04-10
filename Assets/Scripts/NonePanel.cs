using UnityEngine;
using UnityEngine.UI;

public class NonePanel : MonoBehaviour, TilePropertiesPanel {
    EditorTile tile;
    Text typeText;

    void Awake() {
        typeText = transform.Find("TypeText").GetComponent<Text>();
    }

    public void Assign(EditorTile tile) {
        this.tile = tile;
        typeText.text = tile.Type.ToString();
    }
}