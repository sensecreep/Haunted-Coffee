using UnityEngine;
using UnityEngine.UI;

public class TabletController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject tabletPanel;
    [SerializeField] private Image recipeImage;

    [Header("Pages")]
    [SerializeField] private Sprite[] recipePages;

    [Header("Optional Controls To Disable")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private int currentPageIndex = 0;
    private bool isTabletOpen = false;

    private float previousTimeScale = 1f;

    private void Start()
    {
        CloseTabletInstantly();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTablet();
        }

        if (!isTabletOpen)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NextPage();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PreviousPage();
        }
    }

    private void ToggleTablet()
    {
        if (isTabletOpen)
            CloseTablet();
        else
            OpenTablet();
    }

    private void OpenTablet()
    {
        isTabletOpen = true;

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        tabletPanel.SetActive(true);

        SetGameplayScriptsEnabled(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentPageIndex = 0;
        UpdatePage();
    }

    private void CloseTablet()
    {
        isTabletOpen = false;

        Time.timeScale = previousTimeScale;

        tabletPanel.SetActive(false);

        SetGameplayScriptsEnabled(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CloseTabletInstantly()
    {
        isTabletOpen = false;

        if (tabletPanel != null)
            tabletPanel.SetActive(false);
    }

    public void NextPage()
    {
        if (recipePages == null || recipePages.Length == 0)
            return;

        currentPageIndex++;

        if (currentPageIndex >= recipePages.Length)
            currentPageIndex = 0;

        UpdatePage();
    }

    public void PreviousPage()
    {
        if (recipePages == null || recipePages.Length == 0)
            return;

        currentPageIndex--;

        if (currentPageIndex < 0)
            currentPageIndex = recipePages.Length - 1;

        UpdatePage();
    }

    private void UpdatePage()
    {
        if (recipePages == null || recipePages.Length == 0)
        {
            Debug.LogWarning("В планшет не добавлены страницы рецептов");
            return;
        }

        recipeImage.sprite = recipePages[currentPageIndex];
    }

    private void SetGameplayScriptsEnabled(bool value)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = value;
        }
    }
}