using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSessionController : MonoBehaviour
{
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

        // Выключаем все игровые MonoBehaviour, кроме UI и этого скрипта
        foreach (MonoBehaviour script in FindObjectsOfType<MonoBehaviour>())
        {
            if (script == this)
                continue;

            // не трогаем UI
            if (script.GetComponentInParent<Canvas>() != null)
                continue;

            // не трогаем EventSystem, чтобы кнопки EndScreen работали
            if (script is EventSystem)
                continue;

            if (script is BaseInputModule)
                continue;

            script.enabled = false;
        }
    }
}