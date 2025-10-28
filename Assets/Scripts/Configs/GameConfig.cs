using UnityEngine;

[CreateAssetMenu()]
public class GameConfig: ScriptableObject
{
    public float cell_size = 1;
    public float tick_duration = 1;
    public float catastrophe_speed = 0.2f;
    public float[] time_scale_levels = new float[] { 1, 2, 5 };
    public LevelChunk[] chunk_prefabs;
}

