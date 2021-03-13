using UnityEngine;
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

    void Update() {
        var speedMod = CameraSpeed * cam.orthographicSize * Time.deltaTime;
        transform.position += Input.GetAxisRaw("Horizontal") * Vector3.right * CameraSpeed * speedMod;
        transform.position += Input.GetAxisRaw("Vertical") * Vector3.up * speedMod;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y * ZoomSpeed, MinSize, MaxSize);
    }
}