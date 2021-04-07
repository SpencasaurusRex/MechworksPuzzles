using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameData : SerializedMonoBehaviour {
    public static GameData Instance { get; set; }
    void Awake() {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }
    
    public Dictionary<GridType, Sprite> GridSprites;
}