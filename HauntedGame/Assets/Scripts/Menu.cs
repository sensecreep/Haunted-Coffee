using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string introSceneName = "IntroScene";
    [SerializeField] private string selectSlotSceneName = "SelectSlot";
    [SerializeField] private string settingsSceneName = "Settings 1";
    [SerializeField] private string mainMenuSceneName = "StartMenu";
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Save Slots")]
    [SerializeField] private int maxSaveSlots = 3;

    [Header("UI Warning")]
    [SerializeField] private GameObject noFreeSlotsPanel;
    [SerializeField] private TMP_Text noFreeSlotsText;

    private void Start()
    {
        Time.timeScale = 1f;

        if (noFreeSlotsPanel != null)
            noFreeSlotsPanel.SetActive(false);
    }

    // Можно оставить старую привязку кнопки New Game к StartGame()
    public void StartGame()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        int freeSlot = FindFirstEmptySlot();

        if (freeSlot == -1)
        {
            ShowNoFreeSlotsWarning();
            return;
        }

        SaveSystem.SelectedSlot = freeSlot;

        SaveData newSave = new SaveData
        {
            currentDay = 1,
            totalMoney = 0,
            hasSeenIntro = false
        };

        SaveSystem.Save(newSave, freeSlot);

        SceneManager.LoadScene(introSceneName);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(selectSlotSceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void StartSelectSlot()
    {
        ContinueGame();
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene(settingsSceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void HideNoFreeSlotsWarning()
    {
        if (noFreeSlotsPanel != null)
            noFreeSlotsPanel.SetActive(false);
    }

    private int FindFirstEmptySlot()
    {
        for (int slot = 1; slot <= maxSaveSlots; slot++)
        {
            SaveData data = SaveSystem.Load(slot);

            if (data == null)
                return slot;
        }

        return -1;
    }

    private void ShowNoFreeSlotsWarning()
    {
        if (noFreeSlotsText != null)
        {
            noFreeSlotsText.text = "Нет свободных слотов сохранения.\nОсвободите слот в меню продолжения игры.";
        }

        if (noFreeSlotsPanel != null)
            noFreeSlotsPanel.SetActive(true);
        else
            Debug.LogWarning("Нет свободных слотов сохранения.");
    }

    public void LoadNextDay()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(gameSceneName);
    }

    public void StartMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}