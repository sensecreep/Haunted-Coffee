using System.Collections;
using UnityEngine;

public class MilkPourController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pitcher;
    [SerializeField] private Transform pitcherPourSlot;
    [SerializeField] private Transform pourPoint;
    [SerializeField] private Transform cupTarget;
    [SerializeField] private LineRenderer milkStream;
    [SerializeField] private GameObject milkVisualInCup;

    [Header("Pour Settings")]
    [SerializeField] private float pourDuration = 1.5f;
    [SerializeField] private float tiltAngle = 55f;

    [Header("Milk Pour Sound")]
    [SerializeField] private AudioSource milkPourAudioSource;
    [SerializeField] private AudioClip milkPourSound;
    [SerializeField, Range(0f, 1f)] private float milkPourVolume = 1f;

    [Header("Cup Data")]
    [SerializeField] private CupController cup;

    private Quaternion startRotation;
    private bool isPouring = false;

    private void Start()
    {
        if (milkStream != null)
            milkStream.gameObject.SetActive(false);

        if (milkVisualInCup != null)
            milkVisualInCup.SetActive(false);

        if (milkPourAudioSource == null)
            milkPourAudioSource = GetComponent<AudioSource>();

        if (milkPourAudioSource != null)
        {
            milkPourAudioSource.playOnAwake = false;
            milkPourAudioSource.loop = true;
            milkPourAudioSource.volume = milkPourVolume;

            if (milkPourSound != null)
                milkPourAudioSource.clip = milkPourSound;
        }
    }

    public void StartPourMilk()
    {
        if (isPouring)
            return;

        if (cup == null)
        {
            Debug.LogError("CupController не назначен в MilkPourController");
            return;
        }

        if (pitcher == null)
        {
            Debug.LogError("Pitcher не назначен в MilkPourController");
            return;
        }

        if (pitcherPourSlot == null)
        {
            Debug.LogError("PitcherPourSlot не назначен в MilkPourController");
            return;
        }

        pitcher.SetParent(pitcherPourSlot);
        pitcher.localPosition = Vector3.zero;
        pitcher.localRotation = Quaternion.identity;

        startRotation = pitcher.localRotation;

        StartCoroutine(PourMilkRoutine());
    }

    private IEnumerator PourMilkRoutine()
    {
        isPouring = true;

        if (milkStream != null)
            milkStream.gameObject.SetActive(true);

        PlayMilkPourSound();

        float timer = 0f;

        Quaternion targetRotation = startRotation * Quaternion.Euler(tiltAngle, 0f, 0f);

        while (timer < pourDuration)
        {
            timer += Time.deltaTime;
            float t = timer / pourDuration;

            if (pitcher != null)
                pitcher.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            UpdateMilkStream();

            yield return null;
        }

        if (milkVisualInCup != null)
            milkVisualInCup.SetActive(true);

        timer = 0f;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.5f;

            if (pitcher != null)
                pitcher.localRotation = Quaternion.Lerp(targetRotation, startRotation, t);

            UpdateMilkStream();

            yield return null;
        }

        if (pitcher != null)
            pitcher.localRotation = startRotation;

        if (milkStream != null)
            milkStream.gameObject.SetActive(false);

        StopMilkPourSound();

        PitcherController pitcherController = pitcher.GetComponent<PitcherController>();

        if (pitcherController != null)
            pitcherController.ResetToDefault();

        isPouring = false;

        Debug.Log("Молоко добавлено в чашку");
    }

    private void UpdateMilkStream()
    {
        if (milkStream == null || pourPoint == null || cupTarget == null)
            return;

        milkStream.SetPosition(0, pourPoint.position);
        milkStream.SetPosition(1, cupTarget.position);
    }

    private void PlayMilkPourSound()
    {
        if (milkPourAudioSource == null)
            return;

        milkPourAudioSource.Stop();
        milkPourAudioSource.loop = true;
        milkPourAudioSource.volume = milkPourVolume;

        if (milkPourSound != null)
            milkPourAudioSource.clip = milkPourSound;

        milkPourAudioSource.Play();
    }

    private void StopMilkPourSound()
    {
        if (milkPourAudioSource == null)
            return;

        milkPourAudioSource.Stop();
    }
    private void OnDisable()
    {
        StopMilkPourSound();
    }
}