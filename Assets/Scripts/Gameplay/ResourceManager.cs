using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public enum ResourceType
{
    Wood = 0, 
    Energy = 1, 
    Population = 2, 
    PopulationMax = 3,
    Worker = 4,
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
    public float population_gain_interval = 2;
    private float population_gain_time = 0;
    private int[] gain_per_tick;

    public TMPro.TMP_Text resource_display;
    public static ResourceManager instance;
    public string resource_display_format = "<sprite=0>{0} <sprite=1>{1} <sprite=2>{2}/{3}";

    private List<ResourceGainVFXState> resource_gain_vfx = new List<ResourceGainVFXState>();

    public PrefabPool resource_gain_vfx_pool;
    public float resource_gain_vfx_duration = 0.5f;
    public float3 resource_gain_vfx_offset = new float3(0, 1, 0);
    public int generator_count = 0;
    public GameObject gameover_menu;

    public void AddGenerator()
    {
        generator_count++;
    }

    public void RemoveGenerator()
    {
        generator_count--;
        if(generator_count <= 0)
        {
            gameover_menu.SetActive(true);
            TimeManager.instance.Pause();
        }
    }

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
        population_gain_time += Time.deltaTime * TimeManager.instance.time_scale;
        if(population_gain_time >= population_gain_interval)
        {
            population_gain_time -= population_gain_interval;
            int population_direction = math.sign(stock[(int)ResourceType.PopulationMax] - stock[(int)ResourceType.Population]);
            stock[(int)ResourceType.Population] += population_direction;
            stock[(int)ResourceType.Worker] += population_direction;
        }
        string[] stock_text = new string[stock.Length];
        for(int i=0; i<stock.Length; i++)
        {
            stock_text[i] = stock[i].ToString();
        }
        resource_display.text = string.Format(resource_display_format, stock_text);
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

    public void AddResource(ResourceQuantity quantity, float3 pos)
    {
        stock[(int)quantity.resource] += quantity.quantity;
        AddResourceGainVFX(quantity, pos);
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
