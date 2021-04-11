using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPanel : MonoBehaviour, TilePropertiesPanel {
    EditorTile tile;
    Dropdown colorDropdown;
    Text typeText;

    void Awake() {
        typeText = transform.Find("TypeText").GetComponent<Text>();
        colorDropdown = transform.Find("ColorDropdown").GetComponent<Dropdown>();
    }

    public void Assign(EditorTile tile) {
        this.tile = tile;
        typeText.text = tile.Type.ToString();
        
        ColorTileInfo info = tile.Data as ColorTileInfo;
        colorDropdown.value = (int)info.Color;
    }

    public void UpdateDropdown() {
        ObjectColor color = (ObjectColor)colorDropdown.value;
        
        ColorTileInfo info = tile.Data as ColorTileInfo;
        info.Color = color;

        tile.GetComponent<SpriteRenderer>().color = GameData.Instance.Colors[(int)color];
    }
}