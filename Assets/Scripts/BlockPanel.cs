using UnityEngine;
using UnityEngine.UI;

public class BlockPanel : MonoBehaviour
{
    // Configuration
    public BlockInfo[] BlockInfos;
    public RectTransform BlockInfoPanelPrefab;

    void Start() {
        foreach (var info in BlockInfos) {
            var blockInfoPanel = Instantiate(BlockInfoPanelPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<BlockInfoPanel>();
            blockInfoPanel.Setup(info);
        }
    }

    void Update() {
        
    }
}
