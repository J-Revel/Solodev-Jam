using System.Text;
using UnityEngine;

public class BuildingDisplayWidget : MonoBehaviour
{
    public BuildingConfig config;
    public TMPro.TMP_Text title_text;
    public TMPro.TMP_Text description_text;
    public TMPro.TMP_Text building_cost_text;
    public string cost_prefix = "Cost: ";
    public string missing_resource_color = "#ff0000ff";

    void Start()
    {
        if(config)
            UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (title_text)
            title_text.text = config.title;
        if (description_text)
            description_text.text = config.description;
        StringBuilder builder = new StringBuilder(128);
        if(building_cost_text)
        {
            builder.Append(cost_prefix);
            foreach(var cost in config.cost)
            {
                bool error_color = !ResourceManager.instance.CanPay(cost);
                if(error_color)
                {
                    builder.Append("<color=");
                    builder.Append(missing_resource_color);
                    builder.Append(">");
                }
                builder.Append(cost.quantity);
                builder.Append("<sprite=");
                builder.Append((int)cost.resource);
                builder.Append("> ");
                if(error_color)
                    builder.Append("</color>");
            }
            building_cost_text.text = builder.ToString();
        }
    }

    void Update()
    {
        UpdateDisplay();
    }
}
