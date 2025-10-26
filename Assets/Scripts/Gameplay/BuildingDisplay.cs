using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;

[ExecuteAlways]
public class BuildingDisplay : MonoBehaviour
{
    private BuildingBase building_base;
    public SpriteRenderer sprite_renderer;

    public void Start()
    {
        building_base = GetComponentInParent<BuildingBase>();
        sprite_renderer.sprite = building_base.config.display_sprite;
        transform.localPosition = new float3(building_base.config.display_offset, 0);
        transform.localScale = Vector3.one * building_base.config.scale;
        if(building_base.config.production.Length > 0)
            TimeManager.instance.tick_event += OnTick;
    }

    private void OnDestroy()
    {
        if(building_base.config.production.Length > 0)
            TimeManager.instance.tick_event -= OnTick;
    }

    public void Update()
    {
        if (!Application.isPlaying)
        {
            if (building_base == null)
            {

            }
            if (building_base.config != null)
            {
                sprite_renderer.sprite = building_base.config.display_sprite;
                transform.localPosition = new float3(building_base.config.display_offset, 0);
                transform.localScale = Vector3.one * building_base.config.scale;
            }
        }
    }

    public void OnTick()
    {
        foreach (ResourceProduction production in building_base.config.production)
        {
            ResourceQuantity quantity = production.default_production;
            if(production.altitude_bonus.bonus_per_step != 0)
            {
                int y = GridManager.instance.GetCellAtPosition(((float3)transform.position).xy).y;
                int altitude_step = (y- production.altitude_bonus.min_altitude) / production.altitude_bonus.step_height;
                altitude_step = math.min(altitude_step, production.altitude_bonus.max_step);
                quantity.quantity += altitude_step * production.altitude_bonus.bonus_per_step;
            }
            ResourceManager.instance.AddResource(quantity, transform.position);
        }
    }

}
