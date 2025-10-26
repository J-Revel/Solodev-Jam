using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PrefabPool
{
    public GameObject prefab;
    private List<GameObject> used_displays = new List<GameObject>();
    private List<GameObject> visible_displays = new List<GameObject>();
    private List<GameObject> asleep_displays = new List<GameObject>();

    public T Pick<T>(float3 position) where T: Component
    {
        GameObject display = null;
        if (visible_displays.Count > 0)
        {
            int index = visible_displays.Count - 1;
            display = visible_displays[index];
            visible_displays.RemoveAt(index);
        }
        else if (asleep_displays.Count > 0)
        {
            int index = asleep_displays.Count - 1;
            display = asleep_displays[index];
            asleep_displays.RemoveAt(index);
            display.SetActive(true);
        }
        else display = MonoBehaviour.Instantiate(prefab, position, quaternion.identity);
        used_displays.Add(display);
        display.transform.position = position;
        return display.GetComponent<T>();
    }

    public void EndFrame()
    {
        foreach(GameObject display in visible_displays)
        {
            display.SetActive(false);
        }
        asleep_displays.AddRange(visible_displays);
        visible_displays.Clear();
        visible_displays.AddRange(used_displays);
        used_displays.Clear();
    }
}

public class PlayerInputManager : MonoBehaviour
{
    public BuildingConfig selected_building;
    public GameConfig grid_config;
    public PrefabPool cell_cross_pool;
    public PrefabPool ghost_tiles_pool;
    public BuildingBase building_prefab;
    public SpriteRenderer ghost;

    public static PlayerInputManager instance;
    private float2 previous_mouse_pos;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        float2 mouse_pos = Mouse.current.position.ReadValue();

        Ray mouse_ray = Camera.main.ScreenPointToRay(new float3(mouse_pos, 0));
        Ray previous_mouse_ray = Camera.main.ScreenPointToRay(new float3(previous_mouse_pos, 0));
        previous_mouse_pos = mouse_pos;

        Plane plane = new Plane(new float3(0, 0, 1), 0);
        float2 previous_mouse_world_pos = float2.zero;
        float2 mouse_world_pos = float2.zero;
        float enter;
        if (plane.Raycast(previous_mouse_ray, out enter))
        {
            previous_mouse_world_pos = ((float3)previous_mouse_ray.GetPoint(enter)).xy;
        }
        if (plane.Raycast(mouse_ray, out enter))
        {
            mouse_world_pos = ((float3)mouse_ray.GetPoint(enter)).xy;
        }
        int2 mouse_cell = GridManager.instance.GetCellAtPosition(mouse_world_pos);
        float2 mouse_delta = mouse_world_pos - previous_mouse_world_pos;
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            if(selected_building == null)
            {
                if(Mouse.current.rightButton.wasPressedThisFrame)
                {
                    if (GridManager.instance.occupancy.ContainsKey(mouse_cell) && GridManager.instance.occupancy[mouse_cell] != null)
                        GridManager.instance.occupancy[mouse_cell].on_kill?.Invoke();
                }
            }
            else
            {
                selected_building = null;
                ghost.gameObject.SetActive(false);
            }
        }
        if(selected_building != null)
        {
            float2 cell_pos = (float2)mouse_cell * grid_config.cell_size;
            NativeArray<int2> error_cells, good_cells;
            ghost.transform.position = new float3(cell_pos + selected_building.display_offset, 0);
            bool can_place_building = GridManager.instance.CanPlaceBuilding(mouse_cell, selected_building, out good_cells, out error_cells);
            foreach(int2 error_cell in error_cells)
            {
                cell_cross_pool.Pick<Transform>(new float3((float2)error_cell, 0));
            }
            foreach(int2 good_cell in good_cells)
            {
                ghost_tiles_pool.Pick<Transform>(new float3((float2)good_cell, 0));
            }

            if(Mouse.current.leftButton.wasPressedThisFrame 
                && selected_building != null
                && can_place_building
                && ResourceManager.instance.Pay(selected_building.cost))
            {
                BuildingBase prefab = building_prefab;
                if(selected_building.prefab_override != null)
                    prefab = selected_building.prefab_override;
                BuildingBase building = Instantiate(prefab, new float3(cell_pos, 0), quaternion.identity);
                building.config = selected_building;
                building.cell = mouse_cell;
            }
        }
        else
        {
            if(Mouse.current.leftButton.isPressed)
            {
                CameraController.instance.Move(mouse_delta);
            }
        }
    }
    private void LateUpdate()
    {
        cell_cross_pool.EndFrame();
        ghost_tiles_pool.EndFrame();
    }

    public void SelectBuilding(BuildingConfig building)
    {
        selected_building = building;
        ghost.sprite = building.display_sprite;
        ghost.transform.localScale = building.scale * Vector3.one;
        ghost.gameObject.SetActive(true);
        
    }

    public void Move(float2 delta)
    {
        
    }
}
