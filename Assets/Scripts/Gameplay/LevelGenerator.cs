using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameConfig game_config;
    public float chunk_interval = 20;
    public float chunk_generation_overhead = 50;
    private int cursor = 0;
    private int section_cursor = 0;
    private int cursor_in_section = 0;

    void Start()
    {
               
    }

    public LevelChunk PickRandomPrefab(ChunkSection section)
    {
        float probability_sum = 0;
        foreach(LevelChunk chunk in section.prefabs)
        {
            probability_sum += chunk.probability;
        }
        float random = UnityEngine.Random.Range(0, probability_sum);
        foreach(LevelChunk chunk in section.prefabs)
        {
            if(random < chunk.probability)
            {
                return chunk;
            }
            random -= chunk.probability;
        }

        return null;
    }

    void Update()
    {
        float camera_x = Camera.main.transform.position.x - transform.position.x;
        while(camera_x + chunk_generation_overhead > cursor * chunk_interval)
        {
            ChunkSection section = game_config.chunk_sections[math.min(section_cursor, game_config.chunk_sections.Length-1)];
            LevelChunk chunk = PickRandomPrefab(section);
            Instantiate(chunk, (float3)transform.position + new float3(cursor * chunk_interval, 0, 0), quaternion.identity, transform);
            cursor++;
            cursor_in_section++;
            if (cursor_in_section >= section.chunk_count)
            {
                cursor_in_section = 0;
                section_cursor++;
            }
        }
    }
}
