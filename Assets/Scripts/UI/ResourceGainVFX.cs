using Unity.Mathematics;
using UnityEngine;

public class ResourceGainVFX : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public ResourceType resource;
    public int quantity;
    public float3 start_pos;
    public float3 movement_offset = new float3(0, 5, 0);
    public AnimationCurve movement_curve;
    public AnimationCurve color_curve;
    private float anim_ratio = 0;
    public float anim_duration = 0.5f;

    private void Update()
    {
        transform.position = start_pos + movement_offset * movement_curve.Evaluate(anim_ratio);
        anim_ratio += Time.deltaTime / anim_duration;
        text.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), color_curve.Evaluate(anim_ratio));
    }
}
