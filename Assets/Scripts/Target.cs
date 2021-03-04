using UnityEngine;

public class Target : MonoBehaviour
{
    // Configuration
    public int GoalCount;
    public ObjectColor GoalColor;

    // Runtime
    Vector2Int Location;

    int currentCount;

    void Start() {
        var controller = FindObjectOfType<GameController>();
        controller.OnTick += Tick;
        Location = transform.position.xy().RoundToInt();

        var sr = GetComponent<SpriteRenderer>();
        switch (GoalColor) {
            case ObjectColor.Red:
                sr.color = controller.Red;
                break;
            case ObjectColor.Blue:
                sr.color = controller.Blue;
                break;
            case ObjectColor.Green:
                sr.color = controller.Green;
                break;
        }
    }

    void Tick() {
        var go = GameController.Instance.GetGridObject(Location);
        if (go != null) {
            var box = go.GetComponent<Box>();
            if (box != null && box.Color == GoalColor) {
                Destroy(go.gameObject);
                currentCount++;
            }
        }
    }
}
