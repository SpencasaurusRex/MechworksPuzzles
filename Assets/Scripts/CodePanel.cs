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
        text.text = robot.Code;

        GameController.Instance.OnTick2 += Tick;
        targetHighlight = HighlightPanel.anchoredPosition;
    }

    int i = 0;

    void Tick() {
        targetHighlight = new Vector2(targetHighlight.x, LineOffset * i);
        i = robot.CommandIndex;
    }

    void Update() {
        float t = GameController.Instance.PercentComplete; //Mathf.Clamp01(4 * GameController.Instance.PercentComplete - 3);
        HighlightPanel.anchoredPosition = Vector2.Lerp(HighlightPanel.anchoredPosition, targetHighlight, t);
    }
}