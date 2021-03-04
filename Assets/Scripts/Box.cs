using UnityEngine;

public class Box : MonoBehaviour {
    public ObjectColor Color;

    void Start() {
        var sr = GetComponent<SpriteRenderer>();
        switch (Color) {
            case ObjectColor.Red:
                sr.color = GameController.Instance.Red;
                break;
            case ObjectColor.Blue:
                sr.color = GameController.Instance.Blue;
                break;
            case ObjectColor.Green:
                sr.color = GameController.Instance.Green;
                break;
        }
    }
}
