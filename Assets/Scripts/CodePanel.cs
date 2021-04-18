using UnityEngine;
using UnityEngine.UI;

public class CodePanel : MonoBehaviour {
    // Configuration
    public Robot robot;
    public float LineOffset = -29.1f;
    public RectTransform HighlightPanel;

    // Runtime
    Text text;
    Vector2 targetHighlight;

    void Start() {
        text = GetComponentInChildren<Text>();

        GameController.Instance.OnTick2 += Tick;
        targetHighlight = HighlightPanel.anchoredPosition;
    }

    int i = 0;

    void Tick() {
        if (robot && robot.Commands != null) {
            foreach (var command in robot.Commands) {
                text.text += command.ToText() + "\n";
            }
            i = robot.CommandIndex;
        }
        
        targetHighlight = new Vector2(targetHighlight.x, LineOffset * i);
    }

    void Update() {
        float t = GameController.Instance.PercentComplete; //Mathf.Clamp01(4 * GameController.Instance.PercentComplete - 3);
        HighlightPanel.anchoredPosition = Vector2.Lerp(HighlightPanel.anchoredPosition, targetHighlight, t);
    }
}