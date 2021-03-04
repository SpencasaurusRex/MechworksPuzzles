using UnityEngine;

public class SquareGrid : MonoBehaviour
{
    void Start()
    {
        var controller = FindObjectOfType<GameController>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var size = spriteRenderer.size.xy().ToInt();
        int sx = Mathf.RoundToInt(transform.position.x) - size.x / 2;
        int sy = Mathf.RoundToInt(transform.position.y) - size.y / 2;

        for (int y = sy; y < sy + size.y; y++) {    
            for (int x = sx; x < sx + size.x; x++) {
                controller.SetValidSpace(new Vector2Int(x, y));
            }
        }
    }
}
