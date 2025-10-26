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
        foreach(OccupancyCell occupancy in config.occupancy_cells)
        {
            int2 occupancy_cell = cell + occupancy.cell;
            if(occupancy.type.HasFlag(CellType.Occupancy))
            {
                GridManager.instance.occupancy[occupancy_cell] = this;
            }
            if(occupancy.type.HasFlag(CellType.Support))
            {
                GridManager.instance.support[occupancy_cell] = this;
            }
            if(occupancy.type.HasFlag(CellType.NeedSupport))
            {
                GridManager.instance.RegisterCellDelegate(occupancy_cell, OnCellUpdate);
            }
            if(occupancy.type.HasFlag(CellType.KeepEmpty))
            {
                ref var keep_empty = ref GridManager.instance.keep_empty;
                if(!keep_empty.ContainsKey(occupancy_cell))
                    keep_empty.Add(occupancy_cell, new System.Collections.Generic.List<BuildingBase>());
                keep_empty[occupancy_cell].Add(this);
            }
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
        foreach(OccupancyCell occupancy in config.occupancy_cells)
        {
            GridManager.instance.occupancy.Remove(cell + occupancy.cell);
            if(occupancy.type.HasFlag(CellType.Support))
            {
                int2 support_cell = cell + occupancy.cell;
                GridManager.instance.support.Remove(support_cell);
                GridManager.instance.TriggerCellUpdate(support_cell);
            }
            if(occupancy.type.HasFlag(CellType.NeedSupport))
            {
                int2 support_cell = cell + occupancy.cell - new int2(0, 1);
                GridManager.instance.RemoveCellDelegate(support_cell, OnCellUpdate);
            }
        }
        GridManager.instance.RemoveInfluence(cell.x, config.building_influence);
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
