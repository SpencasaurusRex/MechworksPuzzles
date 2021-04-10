using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileSelect : MonoBehaviour, IPointerClickHandler {
    // Configuration
    public Color DefaultColor;
    public Color SelectedColor;

    // Runtime
    public TileSelectInfo info;
    TileSelectPanel panel;

    public void SetSelected(bool selected) {
        var image = GetComponent<Image>();
        if (selected) {
            image.color = SelectedColor;
        }
        else {
            image.color = DefaultColor;
        }
    }

    public void Setup(TileSelectInfo info, TileSelectPanel blockPanel) {
        transform.Find("BlockSprite").GetComponent<Image>().sprite = info.Sprite;
        this.info = info;
        panel = blockPanel;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            panel.SelectTile(this);
        }
    }
}