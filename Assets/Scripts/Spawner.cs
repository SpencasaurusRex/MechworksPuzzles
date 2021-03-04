using UnityEngine;

public class Spawner : MonoBehaviour {
    // Configuration
    public ObjectColor Color;

    public GameObject RedBoxPrefab;
    public GameObject GreenBoxPrefab;
    public GameObject BlueBoxPrefab;

    // Runtime
    Vector2Int Location;

    void Start() {
        var controller = FindObjectOfType<GameController>();
        controller.OnTick += Tick;
        Location = transform.position.xy().RoundToInt();
        Create();

        var sr = GetComponent<SpriteRenderer>();
        switch (Color) {
            case ObjectColor.Red:
                sr.color = controller.Red;
                break;
            case ObjectColor.Green:
                sr.color = controller.Green;
                break;
            case ObjectColor.Blue:
                sr.color = controller.Blue;
                break;
        }
    }

    void Tick() {
        Create();
    }

    void Create() {
        if (GameController.Instance.GetGridObject(Location) == null)
        {
            GameObject prefab;
            if (Color == ObjectColor.Red) prefab = RedBoxPrefab;
            else if (Color == ObjectColor.Green) prefab = GreenBoxPrefab;
            else prefab = BlueBoxPrefab;
            Instantiate(prefab, Location.ToFloat(), Quaternion.identity);
        }
    }
}
