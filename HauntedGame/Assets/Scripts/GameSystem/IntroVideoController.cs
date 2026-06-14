using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroVideoController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Skip")]
    public bool allowSkip = true;
    public KeyCode skipKey = KeyCode.Space;
    public KeyCode secondSkipKey = KeyCode.Escape;

    [Header("Fallback")]
    public float prepareTimeout = 5f;

    private bool isFinishing;

    private IEnumerator Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (videoPlayer == null)
            videoPlayer = FindObjectOfType<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("IntroVideoController: VideoPlayer не назначен и не найден на сцене.");
            FinishIntro();
            yield break;
        }

        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.errorReceived += OnVideoError;

        videoPlayer.Prepare();

        float timer = 0f;
        while (!videoPlayer.isPrepared && timer < prepareTimeout)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        videoPlayer.Play();
    }

    private void Update()
    {
        if (!allowSkip || isFinishing)
            return;

        if (Input.GetKeyDown(skipKey) || Input.GetKeyDown(secondSkipKey))
            FinishIntro();
    }

    private void OnDestroy()
    {
        if (videoPlayer == null)
            return;

        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.errorReceived -= OnVideoError;
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        FinishIntro();
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError("Intro video error: " + message);
        FinishIntro();
    }

    public void FinishIntro()
    {
        if (isFinishing)
            return;

        isFinishing = true;

        SaveData data = SaveSystem.Load(SaveSystem.SelectedSlot) ?? new SaveData
        {
            currentDay = 1,
            totalMoney = 0
        };

        data.hasSeenIntro = true;
        SaveSystem.Save(data, SaveSystem.SelectedSlot);

        SceneManager.LoadScene(gameSceneName);
    }
}