using UnityEngine;

[SelectionBase]
public class Block : MonoBehaviour {
    // Configuration
    public ObjectColor Color;

    public Sprite[] SideSprites;
    public Sprite[] CornerSprites;

    // Runtime
    GridObject gridObject; 

    SpriteRenderer sideSprite;
    SpriteRenderer cornerSprite;

    void Start() {
        sideSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sideSprite.color = GameController.Instance.Colors[(int)Color];    
        cornerSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        cornerSprite.color = GameController.Instance.Colors[(int)Color];    
        
        gridObject = GetComponent<GridObject>();
        gridObject.OnConnect += OnConnect;
        gridObject.OnDisconnect += OnDisconnect;
    }

    void OnDestroy() {
        gridObject.OnConnect -= OnConnect;
        gridObject.OnDisconnect -= OnDisconnect;
    }

    void OnConnect(Side side, GridObject go) {
        UpdateSprite();
    }

    void OnDisconnect(Side side, GridObject go) {
        UpdateSprite();
    }

    void UpdateSprite() {
        int sideIndex = 0;
        int count = 1;
        for (int i = 0; i < 4; i++) {
            if (gridObject.Connected[i] != null) {
                sideIndex += count;
            }
            count *= 2;
        }
        sideSprite.sprite = SideSprites[sideIndex];
    }
}
