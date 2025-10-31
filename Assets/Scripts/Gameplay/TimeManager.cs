using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public GameConfig game_config;
    public System.Action tick_event;
    public int time_scale_level = 0;

    public Button[] time_scale_level_buttons;
    public GameObject pause_display;

    public float time_scale => game_config.time_scale_levels[time_scale_level];
    public GameObject pause_menu;
    private bool pause_menu_displayed;

    private void Awake()
    {
        instance = this;
    }

    public void Resume()
    {
        pause_menu_displayed = false;
        pause_menu.SetActive(pause_menu_displayed);
        PlayerInputManager.instance.input_enabled = !pause_menu_displayed;

    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game Scene");
    }

    private void Update()
    {
        if(!pause_menu_displayed && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Time.timeScale = (Time.timeScale == 0) ? game_config.time_scale_levels[time_scale_level] : 0;
            pause_display.SetActive(Time.timeScale == 0);
            UpdateButtonsDisplay();
        }
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pause_menu_displayed = !pause_menu_displayed;
            pause_menu.SetActive(pause_menu_displayed);
            if(pause_menu_displayed)
                Time.timeScale = 0;
            PlayerInputManager.instance.input_enabled = !pause_menu_displayed;
        }
    }

    public IEnumerator Start()
    {
        Time.timeScale = 0;
        pause_display.SetActive(true);
        UpdateButtonsDisplay();
        for(int i=0; i<time_scale_level_buttons.Length; i++)
        {
            int index = i;
            time_scale_level_buttons[i].onClick.AddListener(() =>
            {
                time_scale_level = index;
                Time.timeScale = game_config.time_scale_levels[time_scale_level];
                pause_display.SetActive(Time.timeScale == 0);
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
