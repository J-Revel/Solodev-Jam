using JetBrains.Annotations;
using System;
using Unity.Mathematics;
using UnityEngine;

[Flags]
public enum CellType
{
    Empty,
    KeepEmpty = 1,
    Occupancy = 2,
    Support = 4,
    NeedSupport = 8,
}

[System.Serializable]
public struct OccupancyCell
{
    public int2 cell;
    public CellType type;
}

[System.Serializable]
public struct BuildingDetectionRange
{
    public int2 min_offset;
    public int2 max_offset;
    public BuildingConfig[] detected_buildings;
}

[System.Serializable]
public struct AltitudeProductionBonus
{
    public int min_altitude;
    public int step_height;
    public int max_step;
    public int bonus_per_step;
}

[System.Serializable]
public struct ResourceProduction
{
    public ResourceQuantity default_production;
    public AltitudeProductionBonus altitude_bonus;
}

[CreateAssetMenu()]
public class BuildingConfig: ScriptableObject
{
    public OccupancyCell[] occupancy_cells;

    public Sprite icon;
    public string icon_additional_text;
    public Color background_color = Color.white;

    public ResourceQuantity[] cost;
    public ResourceProduction[] production;
    public ResourceQuantity[] instant_gain;
    public ResourceQuantity[] death_gain;

    public Sprite display_sprite;
    public float scale;
    public float2 display_offset;
    public int building_influence;
    public BuildingBase prefab_override;

    public ResourceQuantity[] unlock_cost;
    public BuildingDetectionRange[] avoid_buildings;

    public int area_preview_range;
    public Color area_preview_color;

    public string title;
    [TextArea(3, 5)]
    public string description;
}
