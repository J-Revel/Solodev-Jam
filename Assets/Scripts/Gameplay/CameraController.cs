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
    public float2 camera_y_range = new float2(0, 100);

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
        float3 new_pos = transform.position - new Vector3(delta.x, delta.y, 0);
        if(new_pos.y < camera_y_range.x)
            new_pos.y = camera_y_range.x;
        if (new_pos.y > camera_y_range.y)
            new_pos.y = camera_y_range.y;
        //new_pos = math.clamp(new_pos, new float3(0, camera_y_range.x, 0), new float3(float.MaxValue, camera_y_range.y, float.MaxValue));
        transform.position = new_pos;

    }
}
