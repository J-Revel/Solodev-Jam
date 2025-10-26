using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct OccupancyCell
{
    public int2 cell;
    public bool is_support;
}

[System.Serializable]
public struct BuildingDetectionRange
{
    public int2 min_offset;
    public int2 max_offset;
    public BuildingConfig[] detected_buildings;
}

[CreateAssetMenu()]
public class BuildingConfig: ScriptableObject
{
    public OccupancyCell[] occupancy_cells;

    public int2[] support_cells;
    public Sprite icon;
    public ResourceQuantity[] cost;
    public ResourceQuantity[] gain_on_tick;
    public Sprite display_sprite;
    public float scale;
    public float2 display_offset;
    public int building_influence;
    public BuildingBase prefab_override;

    public ResourceQuantity[] unlock_cost;
    public BuildingDetectionRange[] avoid_buildings;


    public string title;
    [TextArea(3, 5)]
    public string description;
}
