using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    [Header("Настройки")]
    public string menuSceneName = "StartMenu"; // Имя сцены меню
    public string targetSceneName = "Loading"; // Сцена где работает этот скрипт

    void Update()
    {
        // Проверяем текущую сцену и нажатие Esc
        if (SceneManager.GetActiveScene().name == targetSceneName && Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        // Загрузка сцены меню
        SceneManager.LoadScene(menuSceneName);

        // Дополнительно: разблокируем курсор (если был заблокирован)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Остановка времени (если было пауза)
        Time.timeScale = 1f;
    }
}