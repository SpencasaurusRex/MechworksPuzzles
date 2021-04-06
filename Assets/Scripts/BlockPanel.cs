using UnityEngine;

public class BlockPanel : MonoBehaviour
{
    // Configuration
    public BlockInfo[] BlockInfos;
    public RectTransform BlockInfoPanelPrefab;
    public Transform TilesParent;

    void Start() {
        foreach (var info in BlockInfos) {
            var blockInfoPanel = Instantiate(BlockInfoPanelPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<BlockInfoPanel>();
            blockInfoPanel.Setup(info, TilesParent);
        }
    }

    void Update() {
        
    }

}
