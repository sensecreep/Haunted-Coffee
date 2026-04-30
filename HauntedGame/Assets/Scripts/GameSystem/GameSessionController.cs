using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionController : MonoBehaviour
{
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
        isRunning = false;

        Debug.Log("Смена окончена");

        // блокируем игрока
        PlayerLock.Instance.Lock();

        // показываем экран
        if (endScreen != null)
            endScreen.SetActive(true);
    }
}