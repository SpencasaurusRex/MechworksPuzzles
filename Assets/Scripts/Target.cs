using UnityEngine;

public class Target : MonoBehaviour
{
    Vector2Int Location;

    void Start() {
        var controller = FindObjectOfType<GameController>();
        controller.OnTick += Tick;
        Location = transform.position.xy().RoundToInt();
    }

    void Tick() {
        var go = GameController.Instance.GetGridObject(Location);
        if (go != null) {
            Destroy(go.gameObject);
        }
    }
}
