using UnityEngine;

public class Wall : MonoBehaviour
{
    // Configuration
    public Sprite[] WallSprites;

    // Runtime
    GridObject gridObject;

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Wall;
    }

    void Start() {
        int index = 0;
        for (int i = 0; i < 4; i++) {
            index >>= 1;
            var neighborPos = gridObject.Location + Util.ToDelta(i);
            var neighbor = GameController.Instance.GetGridObject(neighborPos);
            if (neighbor != null && neighbor.Type == GridType.Wall) { 
                index += 8;
            }
        }

        GetComponent<SpriteRenderer>().sprite = WallSprites[index];
    }
}