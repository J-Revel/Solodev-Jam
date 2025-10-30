using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildingButton : MonoBehaviour
{
    public BuildingConfig building;
    public BuildingDisplayWidget tooltip_display;
    public Image icon_image;
    public Image locked_image;
    public CanvasGroup tooltip_canvasgroup;
    public float transitions_duration = 0.5f;
    private float transition_time;
    public bool show_tooltip;
    private bool locked = true;
    private Button button;
    
    void Start()
    {
        tooltip_display.config = building;
        icon_image.sprite = building.icon;
        button = GetComponent<Button>();
        if (building.unlock_cost.Length == 0)
        {
            locked = false;
        }
        tooltip_display.locked = locked;
        tooltip_display.UpdateDisplay();
        button.onClick.AddListener(() =>
        {
            if(locked)
            {
                if(ResourceManager.instance.Pay(building.unlock_cost))
                {
                    locked = false;
                    tooltip_display.locked = false;
                    tooltip_display.UpdateDisplay();
                }
            }
            else
                PlayerInputManager.instance.SelectBuilding(building);
        });
    }

    public void SetTooltipVisibility(bool visible)
    {
        show_tooltip = visible;
    }

    void Update()
    {
        bool can_buy = ResourceManager.instance.CanPay(locked ? building.unlock_cost : building.cost);
        button.interactable = can_buy;
        locked_image.enabled = locked;

        if (show_tooltip)
            transition_time += Time.unscaledDeltaTime;
        else transition_time -= Time.unscaledDeltaTime;

        transition_time = math.clamp(transition_time, 0, transitions_duration);
        tooltip_canvasgroup.alpha = transition_time / transitions_duration;
    }
}
