using UnityEngine;
using Unity.Mathematics;

public class CatastropheManager : MonoBehaviour
{
    public static CatastropheManager instance;
    public GameConfig config;
    private float catastrophe_position;
    private float time;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        catastrophe_position = transform.position.x;
        
    }

    void Update()
    {
        time += Time.deltaTime;
        int previous_cell = GridManager.instance.GetCellAtPosition(new float2(catastrophe_position, 0)).x;
        catastrophe_position += config.catastrophe_curve.Evaluate(time / config.max_level_time) * Time.deltaTime * TimeManager.instance.time_scale;
        int new_cell = GridManager.instance.GetCellAtPosition(new float2(catastrophe_position, 0)).x;
        if(previous_cell != new_cell)
        {
            GridManager.instance.DestroyBuildingColumn(new_cell);
        }
        transform.position = new float3(catastrophe_position, Camera.main.transform.position.y, 0);
    }
}
