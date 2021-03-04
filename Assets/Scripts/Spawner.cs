using UnityEngine;

public class Spawner : MonoBehaviour {
    // Configuration
    public GameObject prefab;
    
    // Runtime
    Vector2Int Location;

    void Start() {
        var controller = FindObjectOfType<GameController>();
        controller.OnTick += Tick;
        Location = transform.position.xy().RoundToInt();
    }

    void Tick() {
        if (GameController.Instance.GetGridObject(Location) == null) {
            var go = Instantiate(prefab, Location.ToFloat(), Quaternion.identity);
        }
    }
}
