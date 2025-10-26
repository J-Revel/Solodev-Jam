using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public enum ResourceType
{
    Wood = 0, Energy = 1
}

[System.Serializable]
public struct ResourceQuantity
{
    public ResourceType resource;
    public int quantity;
}

public class ResourceGainVFXState
{
    public float delay;
    public float time;
    public ResourceType type;
    public int quantity;
    public float3 start_position;
}

public class ResourceManager : MonoBehaviour
{
    public float max_vfx_delay = 0.3f;
    public int[] stock;
    private int[] gain_per_tick;

    public TMPro.TMP_Text resource_display;
    public static ResourceManager instance;

    private List<ResourceGainVFXState> resource_gain_vfx = new List<ResourceGainVFXState>();

    public PrefabPool resource_gain_vfx_pool;
    public float resource_gain_vfx_duration = 0.5f;
    public float3 resource_gain_vfx_offset = new float3(0, 1, 0);

    public void GainResource(ResourceQuantity gain, float3 position)
    {
        stock[(int)gain.resource] += gain.quantity;
        resource_gain_vfx.Add(new ResourceGainVFXState
        {
            delay = UnityEngine.Random.Range(0, max_vfx_delay),
            quantity = gain.quantity,
            type = gain.resource,
            start_position = position,
        });
    }

    public void AddResourceGainVFX(ResourceQuantity gain, float3 position)
    {
        resource_gain_vfx.Add(new ResourceGainVFXState
        {
            delay = UnityEngine.Random.Range(0, max_vfx_delay),
            quantity = gain.quantity,
            type = gain.resource,
            start_position = position,
        });
    }

    private void Awake()
    {
        instance = this;
        gain_per_tick = new int[stock.Length];
    }

    void Start()
    {
        TimeManager.instance.tick_event += OnTick;
    }

    void Update()
    {
        System.Text.StringBuilder text_display = new System.Text.StringBuilder(100);
        for(int i=0; i<stock.Length; i++)
        {
            text_display.Append("<sprite=");
            text_display.Append(i);
            text_display.Append(">");
            text_display.Append(stock[i]);
            text_display.Append(" ");
        }
        resource_display.text = text_display.ToString();

        for (int i = resource_gain_vfx.Count-1; i >= 0; i--)
        {
            if (resource_gain_vfx[i].delay > 0)
                resource_gain_vfx[i].delay -= Time.deltaTime;
            else
            {
                resource_gain_vfx[i].time += Time.deltaTime;
                float time_ratio = resource_gain_vfx[i].time / resource_gain_vfx_duration;
                string suffix = "";
                if (resource_gain_vfx[i].quantity > 1)
                    suffix = "x" + resource_gain_vfx[i].quantity;
                TMPro.TMP_Text vfx_text = resource_gain_vfx_pool.Pick<TMPro.TMP_Text>(resource_gain_vfx[i].start_position + resource_gain_vfx_offset * time_ratio);
                vfx_text.text = "<sprite="
                    + (int)resource_gain_vfx[i].type
                    + ">"
                    + suffix;
                vfx_text.alpha = 1 - time_ratio;
                if (resource_gain_vfx[i].time >= resource_gain_vfx_duration)
                {
                    resource_gain_vfx.RemoveAtSwapBack(i);
                }

            }
        }
    }

    private void LateUpdate()
    {
        resource_gain_vfx_pool.EndFrame();
    }

    public void OnTick()
    {
        for (int i = 0; i < stock.Length; i++)
            stock[i] += gain_per_tick[i];
    }

    public void AddResourceGainPerTick(ResourceQuantity quantity)
    {
        gain_per_tick[(int)quantity.resource] += quantity.quantity;
    }

    public void RemoveResourceGainPerTick(ResourceQuantity quantity)
    {
        gain_per_tick[(int)quantity.resource] -= quantity.quantity;
    }

    public bool CanPay(ResourceQuantity[] costs)
    {
        foreach(ResourceQuantity cost in costs)
        {
            if (stock[(int)cost.resource] < cost.quantity)
                return false;
        }
        return true;
    }

    public bool CanPay(ResourceQuantity cost)
    {
        return !(stock[(int)cost.resource] < cost.quantity);
    }

    public bool Pay(ResourceQuantity[] costs)
    {
        foreach(ResourceQuantity cost in costs)
        {
            if (stock[(int)cost.resource] < cost.quantity)
                return false;

        }
        foreach (ResourceQuantity cost in costs)
        {
            stock[(int)cost.resource] -= cost.quantity;
        }
        return true;
    }
}
