using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameConfig game_config;
    public float chunk_interval = 20;
    public float chunk_generation_overhead = 50;
    private int cursor;

    void Start()
    {
               
    }
    public LevelChunk PickRandomPrefab()
    {
        float probability_sum = 0;
        foreach(LevelChunk chunk in game_config.chunk_prefabs)
        {
            probability_sum += chunk.probability;
        }
        float random = UnityEngine.Random.Range(0, probability_sum);
        foreach(LevelChunk chunk in game_config.chunk_prefabs)
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
            LevelChunk chunk = PickRandomPrefab();
            Instantiate(chunk, (float3)transform.position + new float3(cursor * chunk_interval, 0, 0), quaternion.identity, transform);
            cursor++;
        }
    }
}
