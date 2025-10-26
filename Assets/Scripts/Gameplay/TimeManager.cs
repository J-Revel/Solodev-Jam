using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public GameConfig game_config;
    public System.Action tick_event;
    public int time_scale_level = 0;

    public Button[] time_scale_level_buttons;

    public float time_scale => game_config.time_scale_levels[time_scale_level];

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator Start()
    {
        UpdateButtonsDisplay();
        for(int i=0; i<time_scale_level_buttons.Length; i++)
        {
            int index = i;
            time_scale_level_buttons[i].onClick.AddListener(() =>
            {
                time_scale_level = index;
                UpdateButtonsDisplay();
            });
        }
        while(true)
        {
            for(float time = 0; time < game_config.tick_duration; time += Time.deltaTime * game_config.time_scale_levels[time_scale_level])
            {
                yield return null;
            }
            tick_event?.Invoke();
        }
    }

    private void UpdateButtonsDisplay()
    {
        for(int j=0; j<time_scale_level_buttons.Length; j++)
        {
            time_scale_level_buttons[j].interactable = (j != time_scale_level);
        }
    }
}
