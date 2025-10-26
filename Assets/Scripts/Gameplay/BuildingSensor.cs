using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildingSensor : MonoBehaviour
{
    public int detected_count;
    public int2 range;
    public BuildingConfig[] detected_config;
    public ResourceQuantity[] gain_per_detection;
    private BuildingBase building_base;
    public Color area_color;

    void Start()
    {
        building_base = GetComponent<BuildingBase>();
        int2 cell = GridManager.instance.GetCellAtPosition(((float3)transform.position).xy);
        for(int i=-range.x+1;i<range.x; i++)
        {
            for(int j=-range.y+1; j<range.y; j++)
            {
                int2 target_cell = cell + new int2(i, j);
                GridManager.instance.RegisterCellDelegate(target_cell, OnCellChanged);
            }
        }
        UpdateCount();
        TimeManager.instance.tick_event += OnTick;
        building_base.on_kill += OnKill;
    }

    private void OnKill()
    {
        TimeManager.instance.tick_event -= OnTick;
        int2 cell = GridManager.instance.GetCellAtPosition(((float3)transform.position).xy);
        for(int i=-range.x+1;i<range.x; i++)
        {
            for(int j=-range.y+1; j<range.y; j++)
            {
                int2 target_cell = cell + new int2(i, j);
                GridManager.instance.RemoveCellDelegate(target_cell, OnCellChanged);
            }
        }
    }

    void OnCellChanged(int2 cell)
    {
        UpdateCount();
    }

    void UpdateCount()
    {
        int2 cell = GridManager.instance.GetCellAtPosition(((float3)transform.position).xy);
        List<BuildingBase> checked_buildings = new List<BuildingBase>();
        detected_count = 0;
        for (int i = -range.x + 1; i < range.x; i++)
        {
            for (int j = -range.y + 1; j < range.y; j++)
            {
                if(GridManager.instance.occupancy.TryGetValue(cell + new int2(i, j), out BuildingBase neighbour))
                {
                    if(!checked_buildings.Contains(neighbour))
                    {
                        checked_buildings.Add(neighbour);
                        if(detected_config.Contains(neighbour.config))
                        {
                            detected_count++;
                        }
                    }
                }
            }
        }
    }
    public void OnTick()
    {
        foreach (ResourceQuantity quantity in gain_per_detection)
        {
            ResourceManager.instance.GainResource(new ResourceQuantity
            {
                resource = quantity.resource,
                quantity = quantity.quantity * detected_count
            }, transform.position);
        }
    }

    void Update()
    {
        int2 cell = GridManager.instance.GetCellAtPosition(((float3)transform.position).xy);
        AreaDisplayManager.instance.DisplayZone(cell - range, cell + range, area_color);
    }
}
