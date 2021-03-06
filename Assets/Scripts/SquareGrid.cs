using UnityEngine;

public class SquareGrid : MonoBehaviour
{
    void Start()
    {
        var controller = FindObjectOfType<GameController>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var size = spriteRenderer.size;
        var sizeInt = spriteRenderer.size.ToInt();
        int sx = Mathf.FloorToInt(transform.position.x) - Mathf.FloorToInt((size.x - 0.5f) * 0.5f);
        int sy = Mathf.FloorToInt(transform.position.y) - Mathf.FloorToInt((size.y - 0.5f) * 0.5f);
        for (int y = sy; y < sy + size.y; y++) {
            for (int x = sx; x < sx + size.x; x++) {
                controller.SetValidSpace(new Vector3Int(x, y, GridLayer.Ground));
            }
        }
    }
}
