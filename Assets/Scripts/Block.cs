﻿using UnityEngine;

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

    void Awake() {
        gridObject = GetComponent<GridObject>();
        gridObject.Type = GridType.Block;
    }

    void Start() {
        sideSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sideSprite.color = GameController.Instance.Colors[(int)Color];    
        cornerSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        cornerSprite.color = GameController.Instance.Colors[(int)Color];    
        
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

        int cornerIndex = 0;
        count = 1;
        for (int i = 0; i < 4; i++) {
            int j = (i + 1) % 4;
            if (name.Contains("(7)")) {
                print($"Checking {i}, {j}, and {i}->{j}");
            }
            if (gridObject.Connected[i] != null && 
                gridObject.Connected[i].Connected[j] != null && 
                gridObject.Connected[j] != null) {
                cornerIndex += count;
                if (name.Contains("(7)")) {
                    print("True! " + cornerIndex);
                }
            }
            count *= 2;
        }
        sideSprite.sprite = SideSprites[sideIndex];
        cornerSprite.sprite = CornerSprites[cornerIndex];
    }
}
