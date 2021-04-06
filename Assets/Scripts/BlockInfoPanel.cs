using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockInfoPanel : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
    // Configuration
    public GameObject EditorTilePrefab;

    // Runtime
    EditorTile dragging;
    Camera cam;
    BlockInfo info;
    Transform tilesParent;

    void Start() {
        cam = Camera.main;
    }

    public void Setup(BlockInfo info, Transform tilesParent) {
        transform.Find("BlockSprite").GetComponent<Image>().sprite = info.Sprite;
        this.info = info;
        this.tilesParent = tilesParent;
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        dragging = Instantiate(EditorTilePrefab, Vector3.zero, Quaternion.identity, tilesParent).GetComponent<EditorTile>();
        dragging.Setup(info);
    }

    public void OnDrag(PointerEventData eventData) {
        dragging.OnDrag(Camera.main.ScreenToWorldPoint(eventData.position));
    }

    public void OnEndDrag(PointerEventData eventData) {
        dragging.EndDrag(Camera.main.ScreenToWorldPoint(eventData.position));
    }
}