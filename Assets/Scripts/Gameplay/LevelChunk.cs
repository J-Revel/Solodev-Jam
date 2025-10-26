using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelChunk : MonoBehaviour
{
    public int difficulty_level = 0;
    public float probability = 1;
    public new TilemapCollider2D collider;

    private void Start()
    {
        GridManager.instance.tilemap_colliders.Add(collider);
    }
    private void OnDestroy()
    {
        GridManager.instance.tilemap_colliders.Remove(collider);
    }
}
