using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public string volumeParameter = "MasterVolume";

    [Header("UI")]
    public Slider slider;

    private void Start()
    {
        if (slider != null)
        {
            float currentVolume;
            if (audioMixer.GetFloat(volumeParameter, out currentVolume))
            {
                // Convert from dB back to [0,1] range
                slider.value = Mathf.Pow(10f, currentVolume / 20f);
            }

            slider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        // Convert slider [0,1] to decibels [-80,0]
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        audioMixer.SetFloat(volumeParameter, dB);
    }
}
