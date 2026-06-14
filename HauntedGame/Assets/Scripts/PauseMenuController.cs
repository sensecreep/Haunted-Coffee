using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "StartMenu";

    [Header("Canvas")]
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private int pauseCanvasSortingOrder = 999;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject gameplayHUD;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject quitWarningPanel;

    [Header("Goal UI")]
    [SerializeField] private TMP_Text goalProgressText;
    [SerializeField] private int moneyGoal = 50000;

    [Header("Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    [Header("Slider Texts")]
    [SerializeField] private TMP_Text musicVolumeValueText;
    [SerializeField] private TMP_Text mouseSensitivityValueText;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private FirstPersonCamera firstPersonCamera;

    [Header("Input")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused;

    private bool previousPlayerControllerState;
    private bool previousCameraState;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;

    private const string MusicVolumeKey = "MusicVolume";
    private const string MouseSensitivityKey = "MouseSensitivity";

    private void Awake()
    {
        SetupPauseCanvas();
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (pauseCanvas == null)
            pauseCanvas = GetComponentInParent<Canvas>();

        if (musicSource == null)
            musicSource = FindObjectOfType<AudioSource>();

        if (firstPersonCamera == null)
            firstPersonCamera = FindObjectOfType<FirstPersonCamera>();

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (quitWarningPanel != null)
            quitWarningPanel.SetActive(false);

        if (gameplayHUD != null)
            gameplayHUD.SetActive(true);

        LoadSettings();
        UpdateGoalProgressText();

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(pauseKey))
            return;

        if (isPaused)
        {
            ResumeGame();
            return;
        }

        if (IsPauseBlockedByInteraction())
            return;

        PauseGame();
    }

    private void LateUpdate()
    {
        if (isPaused)
        {
            ForcePauseCursor();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        if (IsPauseBlockedByInteraction())
            return;

        isPaused = true;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        if (playerController != null)
        {
            previousPlayerControllerState = playerController.enabled;
            playerController.enabled = false;
        }

        if (firstPersonCamera != null)
        {
            previousCameraState = firstPersonCamera.enabled;
            firstPersonCamera.enabled = false;
        }

        Time.timeScale = 0f;

        ForcePauseCursor();

        if (gameplayHUD != null)
            gameplayHUD.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (quitWarningPanel != null)
            quitWarningPanel.SetActive(false);

        SetupPauseCanvas();
        UpdateGoalProgressText();
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (quitWarningPanel != null)
            quitWarningPanel.SetActive(false);

        if (gameplayHUD != null)
            gameplayHUD.SetActive(true);

        if (playerController != null)
            playerController.enabled = previousPlayerControllerState;

        if (firstPersonCamera != null)
            firstPersonCamera.enabled = previousCameraState;

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisible;
    }

    public void OpenQuitWarning()
    {
        if (quitWarningPanel != null)
            quitWarningPanel.SetActive(true);
    }

    public void CloseQuitWarning()
    {
        if (quitWarningPanel != null)
            quitWarningPanel.SetActive(false);
    }

    public void ConfirmQuitToMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        if (musicVolumeValueText != null)
            musicVolumeValueText.text = "музыка " + Mathf.RoundToInt(value * 100f) + "%";

        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void OnMouseSensitivityChanged(float value)
    {
        if (firstPersonCamera != null)
            firstPersonCamera.sensitivity = value;

        if (mouseSensitivityValueText != null)
            //mouseSensitivityValueText.text = "чувствительность " + Mathf.RoundToInt(value).ToString();
            mouseSensitivityValueText.text = "чувствительность";

        PlayerPrefs.SetFloat(MouseSensitivityKey, value);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.7f);
        float mouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 300f);

        if (musicVolumeSlider != null)
            musicVolumeSlider.SetValueWithoutNotify(musicVolume);

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.SetValueWithoutNotify(mouseSensitivity);

        OnMusicVolumeChanged(musicVolume);
        OnMouseSensitivityChanged(mouseSensitivity);
    }

    private void UpdateGoalProgressText()
    {
        if (goalProgressText == null)
            return;

        int totalEarned = GetTotalEarnedMoney();

        goalProgressText.text = "Цель достигнута на "
            + FormatMoney(totalEarned)
            + " / "
            + FormatMoney(moneyGoal);
    }

    private int GetTotalEarnedMoney()
    {
        int savedTotalMoney = 0;
        int currentDayMoney = 0;

        if (GameProgressManager.Instance != null)
        {
            savedTotalMoney = GameProgressManager.Instance.TotalMoney;
        }
        else
        {
            SaveData data = SaveSystem.Load(SaveSystem.SelectedSlot);

            if (data != null)
                savedTotalMoney = data.totalMoney;
        }

        if (MoneyManager.Instance != null)
            currentDayMoney = MoneyManager.Instance.GetDayMoney();

        return savedTotalMoney + currentDayMoney;
    }

    private string FormatMoney(int amount)
    {
        return amount.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " ");
    }

    private void SetupPauseCanvas()
    {
        if (pauseCanvas == null)
            pauseCanvas = GetComponentInParent<Canvas>();

        if (pauseCanvas == null)
            return;

        pauseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pauseCanvas.overrideSorting = true;
        pauseCanvas.sortingOrder = pauseCanvasSortingOrder;
    }

    private void ForcePauseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private bool IsPauseBlockedByInteraction()
    {
        if (PlayerLock.Instance != null && PlayerLock.Instance.IsLocked)
            return true;

        if (CameraFocusController.Instance != null && CameraFocusController.Instance.IsFocused)
            return true;

        return false;
    }
}