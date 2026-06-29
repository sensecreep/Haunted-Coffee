using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingSceneController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string mainMenuSceneName = "StartMenu";

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer не найден на EndingSceneController");
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        if (videoPlayer.clip == null)
        {
            Debug.LogError("В VideoPlayer не назначен Video Clip");
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}