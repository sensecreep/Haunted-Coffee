using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    private const string MusicVolumeKey = "MusicVolume";
    private const string MouseSensitivityKey = "MouseSensitivity";

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    [Header("Defaults")]
    [SerializeField] private float defaultMusicVolume = 0.5f;
    [SerializeField] private float defaultMouseSensitivity = 300f;

    private void Start()
    {
        SetupMusicSlider();
        SetupMouseSensitivitySlider();
    }

    private void SetupMusicSlider()
    {
        if (musicSlider == null)
            return;

        float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey, defaultMusicVolume);

        musicSlider.minValue = 0f;
        musicSlider.maxValue = 1f;
        musicSlider.wholeNumbers = false;
        musicSlider.value = savedVolume;

        ApplyMusicVolume(savedVolume);

        musicSlider.onValueChanged.RemoveListener(ApplyMusicVolume);
        musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
    }

    private void SetupMouseSensitivitySlider()
    {
        if (mouseSensitivitySlider == null)
            return;

        float savedSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, defaultMouseSensitivity);

        mouseSensitivitySlider.minValue = 50f;
        mouseSensitivitySlider.maxValue = 600f;
        mouseSensitivitySlider.wholeNumbers = false;
        mouseSensitivitySlider.value = savedSensitivity;

        mouseSensitivitySlider.onValueChanged.RemoveListener(ApplyMouseSensitivity);
        mouseSensitivitySlider.onValueChanged.AddListener(ApplyMouseSensitivity);
    }

    private void ApplyMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();

        AudioListener.volume = value;
    }

    private void ApplyMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat(MouseSensitivityKey, value);
        PlayerPrefs.Save();
    }
}