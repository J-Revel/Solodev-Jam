using UnityEngine;
using Unity.Mathematics;

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
        if(building_base.config.gain_on_tick.Length > 0)
            TimeManager.instance.tick_event += OnTick;
    }

    private void OnDestroy()
    {
        if(building_base.config.gain_on_tick.Length > 0)
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
        foreach (ResourceQuantity quantity in building_base.config.gain_on_tick)
        {
            ResourceManager.instance.AddResourceGainVFX(quantity, transform.position);
        }
    }

}
