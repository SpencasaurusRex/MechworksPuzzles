using UnityEngine;

[CreateAssetMenu(fileName = "BlockInfo", menuName = "MechworksPuzzles/BlockInfo", order = 0)]
public class BlockInfo : ScriptableObject {
    public Sprite Sprite;
    public GridType Type;
}