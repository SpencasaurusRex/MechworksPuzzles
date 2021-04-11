using UnityEngine;
using UnityEngine.UI;

public class TargetPanel : MonoBehaviour, TilePropertiesPanel {
    
    EditorTile tile;
    Text typeText;
    Dropdown colorDropdown;
    InputField inputText;

    string goalCountTextValue;

    void Awake() {
        typeText = transform.Find("TypeText").GetComponent<Text>();
        colorDropdown = transform.Find("ColorDropdown").GetComponent<Dropdown>();
        inputText = transform.Find("GoalCountInput").GetComponent<InputField>();
    }

    public void Assign(EditorTile tile) {
        this.tile = tile;
        typeText.text = tile.Type.ToString();
        
        TargetTileInfo info = tile.Data as TargetTileInfo;
        inputText.text = info.GoalCount.ToString();
        colorDropdown.value = (int)info.Color;
    }

    public void OnUpdateText() {
        TargetTileInfo info = tile.Data as TargetTileInfo;
        string newText = inputText.text;
        
        if (byte.TryParse(newText, out var goalCount)) {
            goalCountTextValue = newText;
            info.GoalCount = goalCount;
        }
        else if (string.IsNullOrEmpty(newText)) {
            // Allow empty string
            info.GoalCount = 0;
            goalCountTextValue = newText;
        }
        else {
            inputText.text = goalCountTextValue;
        }
    }

    public void FinishUpdatingText() {
        // Replace empty string with 0
        if (string.IsNullOrEmpty(inputText.text)) {
            inputText.text = "0";
            TargetTileInfo info = tile.Data as TargetTileInfo;
            info.GoalCount = 0;
        }
    }

    public void OnUpdateDropdown() {
        ObjectColor color = (ObjectColor)colorDropdown.value;

        TargetTileInfo info = tile.Data as TargetTileInfo;
        info.Color = color;

        tile.GetComponent<SpriteRenderer>().color = GameData.Instance.Colors[(int)color];
    }
}