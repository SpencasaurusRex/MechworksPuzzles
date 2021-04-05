using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockInfoPanel : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
    // Configuration
    GameObject prefab;
    // Runtime
    EditorTile dragging;
    Camera cam;

    void Start() {
        cam = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        dragging = Instantiate(prefab).GetComponent<EditorTile>();
    }

    public void OnDrag(PointerEventData eventData) {
        dragging.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData) {
        
    }

    public void Setup(BlockInfo info) {
        transform.Find("BlockSprite").GetComponent<Image>().sprite = info.Sprite;
        prefab = info.Prefab;
    }
}