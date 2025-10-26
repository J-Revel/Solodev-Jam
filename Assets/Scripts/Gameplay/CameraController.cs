using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    
    public float zoom;
    public float zoom_increments = 0.1f;
    public float2 fov_range = new float2(60, 30);
    private new Camera camera;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        float scroll = Mouse.current.scroll.value.y * zoom_increments;
        zoom = math.saturate(zoom + scroll);
        camera.fieldOfView = math.lerp(fov_range.x, fov_range.y, zoom);
    }

    public void Move(float2 delta)
    {
        transform.position -= new Vector3(delta.x, delta.y, 0);
    }
}
