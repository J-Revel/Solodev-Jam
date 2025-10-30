using UnityEngine;

[CreateAssetMenu()]
public class GameConfig: ScriptableObject
{
    public float cell_size = 1;
    public float tick_duration = 1;
    public float catastrophe_speed = 0.2f;
    public AnimationCurve catastrophe_curve;
    public float[] time_scale_levels = new float[] { 1, 2, 5 };
    public float max_level_time = 15 * 60;
    public LevelChunk[] chunk_prefabs;
}

