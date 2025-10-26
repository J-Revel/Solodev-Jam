using Unity.Mathematics;
using UnityEngine;

public class AreaDisplayManager : MonoBehaviour
{
    public int2 area_min, area_max;
    public PrefabPool area_pool;

    public static AreaDisplayManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void LateUpdate()
    {
        area_pool.EndFrame();
    }

    public void DisplayZone(int2 cell_min, int2 cell_max, Color color)
    {
        float2 min = GridManager.instance.GetCellPosition(cell_min) - new float2(0.5f, 0.5f);
        float2 max = GridManager.instance.GetCellPosition(cell_max) + new float2(0.5f, 0.5f);
        float2 center = (max + min) / 2;
        float2 size = max - min;
        SpriteRenderer area = area_pool.Pick<SpriteRenderer>(new float3(center, 0));
        area.color = color;
        area.transform.localScale = new float3(size, 1);
    }
}
