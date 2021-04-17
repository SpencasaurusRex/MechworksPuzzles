using UnityEngine;

[SelectionBase]
public class Block : MonoBehaviour {
    // Configuration
    public ColorTileInfo Data;

    public Sprite[] SideSprites;
    public Sprite[] CornerSprites;

    // Runtime
    GridObject gridObject; 

    SpriteRenderer sideSprite;
    SpriteRenderer cornerSprite;

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Block;
    }

    void Start() {
        gridObject.OnConnect += OnConnect;
        gridObject.OnDisconnect += OnDisconnect;
        sideSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        cornerSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    public void AssignData(TileData data) {
        Data = data as ColorTileInfo;
        sideSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sideSprite.color = GameData.Instance.Colors[(int)Data.Color];
        cornerSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        cornerSprite.color = GameData.Instance.Colors[(int)Data.Color];
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
            if (gridObject.Connected[i] != null && gridObject.Connected[i].Type == GridType.Block) {
                sideIndex += count;
            }
            count *= 2;
        }

        int cornerIndex = 0;
        count = 1;
        for (int i = 0; i < 4; i++) {
            int j = (i + 1) % 4;
            if (gridObject.Connected[i] != null &&
                gridObject.Connected[i].Type == GridType.Block &&
                // gridObject.Connected[i].Connected[j] != null && 
                // gridObject.Connected[i].Connected[j].Type == GridType.Block && 
                gridObject.Connected[j] != null &&
                gridObject.Connected[j].Type == GridType.Block) {
                cornerIndex += count;
            }
            count *= 2;
        }
        sideSprite.sprite = SideSprites[sideIndex];
        cornerSprite.sprite = CornerSprites[cornerIndex];
    }
}
