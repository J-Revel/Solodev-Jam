using UnityEngine;

public class BuildingButtonList : MonoBehaviour
{
    public BuildingButton button;
    public BuildingConfig[] buildings;

    void Start()
    {
        foreach (BuildingConfig building in buildings)
            Instantiate(button, transform).building = building;
    }

    void Update()
    {
        
    }
}
