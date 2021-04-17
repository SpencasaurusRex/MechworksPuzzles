using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    // Configuration
    public float CameraSpeed = 1f;
    
    public float ZoomSpeed = .5f;
    public float MinSize = 5f;
    public float MaxSize = 20f;

    // Runtime
    Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
    }

    bool lostFocus;
    bool currentFocus;

    Vector3 LastMouseWorldPosition;

    bool wasdEnabled = true;

    public void SetWASDEnabled(bool enabled) {
        wasdEnabled = enabled;
    }

    void Update() {
        if (EventSystem.current == null) return;
        if (wasdEnabled) {
            var speedMod = CameraSpeed * cam.orthographicSize * Time.deltaTime;
            transform.position += Input.GetAxisRaw("Horizontal") * Vector3.right * CameraSpeed * speedMod;
            transform.position += Input.GetAxisRaw("Vertical") * Vector3.up * speedMod;
        }
        
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y * ZoomSpeed, MinSize, MaxSize);

        if ((Input.GetMouseButton(1) || Input.GetMouseButton(2)) && !lostFocus) {
            Camera.main.transform.position += (LastMouseWorldPosition - Util.MouseWorldPosition(Camera.main));
        }
        
        LastMouseWorldPosition = Util.MouseWorldPosition(Camera.main);
        
        if (currentFocus) {
            lostFocus = false;
        }
    }

    void OnApplicationFocus(bool focusStatus) {
        currentFocus = focusStatus;
        if (!focusStatus) lostFocus = true;
    }
}