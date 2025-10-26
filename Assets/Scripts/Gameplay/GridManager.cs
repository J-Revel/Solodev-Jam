using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    public GameConfig game_config;
    public Dictionary<int2, BuildingBase> occupancy = new Dictionary<int2, BuildingBase>();
    public Dictionary<int2, BuildingBase> support = new Dictionary<int2, BuildingBase>();
    public Dictionary<int2, System.Action<int2>> cell_update_delegates = new Dictionary<int2, System.Action<int2>>();
    public Dictionary<int, int> influence_columns = new Dictionary<int, int>();
    public List<TilemapCollider2D> tilemap_colliders = new List<TilemapCollider2D>();
    public PrefabPool occlusion_zone_pool;
    public int occlusion_zone_preview_range = 50;
    public int2 y_range = new int2(-50, 50);

    private void Awake()
    {
        instance = this;
    }

    public int2 GetCellAtPosition(float2 position)
    {
        return (int2)math.round(position / game_config.cell_size);
    }

    public float2 GetCellPosition(int2 cell)
    {
        return (float2)cell * game_config.cell_size;
    }

    bool IsCellAvailable(int2 cell)
    {
        bool colliders = false;
        foreach (var tilemap_collider in tilemap_colliders)
        {
            if (tilemap_collider.OverlapPoint(GetCellPosition(cell)))
                colliders = true;
        }
        return !GridManager.instance.occupancy.ContainsKey(cell) 
            && !colliders
            && influence_columns.ContainsKey(cell.x)
            && influence_columns[cell.x] > 0;
    }

    bool IsCellSupport(int2 cell)
    {
        bool colliders = false;
        foreach (var tilemap_collider in tilemap_colliders)
        {
            if (tilemap_collider.OverlapPoint(GetCellPosition(cell)))
                colliders = true;
        }
        return !GridManager.instance.support.ContainsKey(cell) 
            && !colliders
            && influence_columns.ContainsKey(cell.x)
            && influence_columns[cell.x] > 0;
    }

    public bool CanPlaceBuilding(int2 cell, BuildingConfig building, out NativeArray<int2> out_good_cells, out NativeArray<int2> out_error_cells)
    {
        NativeList<int2> error_cells = new NativeList<int2>(Allocator.Temp);
        NativeList<int2> good_cells = new NativeList<int2>(Allocator.Temp);
        foreach(OccupancyCell occupancy in building.occupancy_cells)
        {
            if (!IsCellAvailable(cell + occupancy.cell))
                error_cells.Add(cell + occupancy.cell);
            else
                good_cells.Add(cell + occupancy.cell);
        }
        foreach(int2 cell_offset in building.support_cells)
        {
            if (IsCellSupport(cell + cell_offset))
                error_cells.Add(cell + cell_offset);
            else
                good_cells.Add(cell + cell_offset);
        }
        out_error_cells = error_cells.AsArray();
        out_good_cells = good_cells.AsArray();
        return error_cells.Length == 0;
    }

    public void AddInfluence(int center, int range)
    {
        for(int i=0; i<range; i++)
        {
            if(!influence_columns.ContainsKey(center + i))
                influence_columns.Add(center + i, 0);
            influence_columns[center + i] += 1;
            if(i > 0)
            {
                if(!influence_columns.ContainsKey(center - i))
                    influence_columns.Add(center - i, 0);
                influence_columns[center - i] += 1;
            }
        }
    }
    public void RemoveInfluence(int center, int range)
    {
        for(int i=0; i<range; i++)
        {
            if(!influence_columns.ContainsKey(center + i))
            {
                influence_columns.Add(center + i, 0);
            }
            influence_columns[center + i] -= 1;
            if(i > 0)
            {
                influence_columns[center - i] -= 1;
            }
        }
    }

    public void DestroyBuildingColumn(int column)
    {
        for (int i = y_range.x; i < y_range.y; i++)
        {
            int2 cell = new int2(column, i);
            if(occupancy.Remove(cell, out BuildingBase building))
            {
                building.on_kill();
            }
        }
    }

    public void Update()
    {
        int2 view_center_cell = GetCellAtPosition(((float3)Camera.main.transform.position).xy);
        int start_occlusion_x = 0;
        bool in_occlusion = false;
        for(int i=view_center_cell.x - occlusion_zone_preview_range; i<=view_center_cell.x + occlusion_zone_preview_range; i++)
        {
            bool occlusion = !influence_columns.ContainsKey(i) || influence_columns[i] <= 0;
            if(occlusion != in_occlusion)
            {
                in_occlusion = occlusion;
                if(in_occlusion)
                {
                    start_occlusion_x = i;
                }
                else
                {
                    float start_x = start_occlusion_x;
                    float end_x = i - 1;
                    float center = (start_x + end_x)/2;
                    SpriteRenderer occlusion_zone = occlusion_zone_pool.Pick<SpriteRenderer>(new float3(center, 0, 0));
                    occlusion_zone.transform.localScale = new float3(end_x - start_x + 1, 1000, 1);
                }
            }
        }
        if(in_occlusion)
        {
            float start_x = start_occlusion_x;
            float end_x = view_center_cell.x + occlusion_zone_preview_range;
            float center = (start_x + end_x)/2;
            SpriteRenderer occlusion_zone = occlusion_zone_pool.Pick<SpriteRenderer>(new float3(center, 0, 0));
            occlusion_zone.transform.localScale = new float3(end_x - start_x + 1, 1000, 1);

        }
        occlusion_zone_pool.EndFrame();
    }

    public void RegisterCellDelegate(int2 cell, System.Action<int2> callback)
    {
        if(!cell_update_delegates.ContainsKey(cell))
        {
            cell_update_delegates[cell] = default;
        }
        cell_update_delegates[cell] += callback;
    }

    public void RemoveCellDelegate(int2 cell, System.Action<int2> callback)
    {
        cell_update_delegates[cell] -= callback;
    }
}
