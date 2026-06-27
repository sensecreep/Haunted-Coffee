using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSessionController : MonoBehaviour
{
    [Header("End Screen Stats")]
    public TextMeshProUGUI earnedTodayText;
    public TextMeshProUGUI totalMoneyText;
    public TextMeshProUGUI dayText;

    [Header("Player")]
    public MonoBehaviour playerController;
    public MonoBehaviour cameraController;

    [Header("Time")]
    public float sessionDuration = 120f; // 2 минуты

    private float timer;
    private bool isRunning = true;

    [Header("UI")]
    public RectTransform clockArrow;

    [Header("End Screen")]
    public GameObject endScreen;

    void Start()
    {
        timer = sessionDuration;

        if (endScreen != null)
            endScreen.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        timer -= Time.deltaTime;

        UpdateClock();

        if (timer <= 0f)
        {
            EndSession();
        }
    }

    void UpdateClock()
    {
        float normalized = timer / sessionDuration;

        // 0 → 360 градусов
        float angle = normalized * 360f;

        // вращаем стрелку
        clockArrow.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void EndSession()
    {
        if (!isRunning) return;

        isRunning = false;

        Debug.Log("Смена окончена");

        int earnedToday = MoneyManager.Instance.GetDayMoney();

        int finishedDay = GameProgressManager.Instance.CurrentDay;

        GameProgressManager.Instance.CompleteDay(earnedToday);

        if (earnedTodayText != null)
            earnedTodayText.text = "Выручка за сегодня: " + earnedToday + " руб.";

        if (totalMoneyText != null)
            totalMoneyText.text = "За всё время: " + GameProgressManager.Instance.TotalMoney + " руб.";

        if (dayText != null)
            dayText.text = "Конец " + finishedDay + " смены";

        // Показываем экран конца дня
        if (endScreen != null)
            endScreen.SetActive(true);

        // Останавливаем время
        Time.timeScale = 0f;

        // Курсор для UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Останавливаем всех NavMeshAgent
        foreach (NavMeshAgent agent in FindObjectsOfType<NavMeshAgent>())
        {
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;
        }

        // Останавливаем анимации
        foreach (Animator animator in FindObjectsOfType<Animator>())
        {
            animator.speed = 0f;
        }

        // Отключаем все Canvas, кроме CanvasEndScreen
        foreach (Canvas canvas in FindObjectsOfType<Canvas>(true))
        {
            bool isEndScreenCanvas = endScreen != null &&
                                     (canvas.gameObject == endScreen ||
                                      canvas.transform.IsChildOf(endScreen.transform));

            if (isEndScreenCanvas)
                continue;

            canvas.enabled = false;

            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
                raycaster.enabled = false;
        }

        // Выключаем все игровые MonoBehaviour, кроме EndScreen, EventSystem и этого скрипта
        foreach (MonoBehaviour script in FindObjectsOfType<MonoBehaviour>(true))
        {
            if (script == this)
                continue;

            // не трогаем EventSystem, чтобы кнопки EndScreen работали
            if (script is EventSystem)
                continue;

            if (script is BaseInputModule)
                continue;

            // не трогаем только скрипты, которые находятся внутри EndScreen
            bool isEndScreenScript = endScreen != null &&
                                     script.transform.IsChildOf(endScreen.transform);

            if (isEndScreenScript)
                continue;

            script.enabled = false;
        }
    }
}