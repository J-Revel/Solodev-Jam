using Unity.Mathematics;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    public System.Action on_tick;
    public System.Action on_kill;
    public BuildingConfig config;
    public int2 cell;

    void Start()
    {
        cell = GridManager.instance.GetCellAtPosition(((float3)transform.position).xy);
        foreach(var gain in config.gain_on_tick)
            ResourceManager.instance.AddResourceGainPerTick(gain);
        Debug.Log(transform.position + " " + cell);
        foreach(OccupancyCell occupancy in config.occupancy_cells)
        {
            GridManager.instance.occupancy[cell + occupancy.cell] = this;
            if(occupancy.is_support)
            {
                GridManager.instance.support[cell + occupancy.cell] = this;
            }
        }
        foreach(int2 supporting_cell in config.support_cells)
        {
            int2 world_cell = cell + supporting_cell;
            GridManager.instance.RegisterCellDelegate(world_cell, OnCellUpdate);
        }
        on_kill += OnKill;
        GridManager.instance.AddInfluence(cell.x, config.building_influence);
    }

    void OnCellUpdate(int2 cell)
    {
        if (!GridManager.instance.support.ContainsKey(cell))
        {
            on_kill?.Invoke();
        }
    }


    private void OnKill()
    {
        int2 cell = GridManager.instance.GetCellAtPosition(((float3)transform.position).xy);
        foreach(int2 supporting_cell in config.support_cells)
        {
            int2 world_cell = cell + supporting_cell;
            GridManager.instance.RemoveCellDelegate(world_cell, OnCellUpdate);
        }
        foreach(var gain in config.gain_on_tick)
            ResourceManager.instance.RemoveResourceGainPerTick(gain);
        foreach(OccupancyCell occupancy in config.occupancy_cells)
        {
            GridManager.instance.occupancy.Remove(cell + occupancy.cell);
            if(occupancy.is_support)
            {
                int2 support_cell = cell + occupancy.cell;
                GridManager.instance.support.Remove(support_cell);
                if(GridManager.instance.cell_update_delegates.ContainsKey(support_cell))
                    GridManager.instance.cell_update_delegates[support_cell]?.Invoke(support_cell);
            }
        }
        GridManager.instance.RemoveInfluence(cell.x, config.building_influence);
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
