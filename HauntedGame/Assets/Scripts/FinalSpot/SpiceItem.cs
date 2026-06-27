using System.Collections;
using UnityEngine;

public class SpiceItem : MonoBehaviour
{
    public AddonType type;

    [Header("UI Name")]
    public string displayName;

    [Header("Pour Animation")]
    public Transform pourSlot;
    public GameObject capObject;

    public float moveToSlotDuration = 0.35f;
    public float tiltDuration = 0.25f;
    public float pourHoldDuration = 0.35f;
    public float returnDuration = 0.35f;

    public Vector3 pourRotationEuler = new Vector3(0f, 0f, -70f);

    [Header("Spice Sound")]
    public AudioSource spiceAudioSource;
    public AudioClip spicePourSound;
    [Range(0f, 1f)] public float spicePourVolume = 1f;

    public bool IsAnimating { get; private set; }

    private void Start()
    {
        if (spiceAudioSource == null)
            spiceAudioSource = GetComponent<AudioSource>();

        if (spiceAudioSource != null)
        {
            spiceAudioSource.playOnAwake = false;
            spiceAudioSource.loop = false;
            spiceAudioSource.volume = spicePourVolume;

            if (spicePourSound != null)
                spiceAudioSource.clip = spicePourSound;
        }
    }

    public IEnumerator PlayPourAnimation(System.Action onPourMoment)
    {
        if (IsAnimating)
            yield break;

        if (pourSlot == null)
        {
            Debug.LogError("У специи " + name + " не назначен Pour Slot");
            yield break;
        }

        IsAnimating = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        Vector3 targetPosition = pourSlot.position;
        Quaternion targetRotation = pourSlot.rotation;

        if (capObject != null)
            capObject.SetActive(false);

        float timer = 0f;

        while (timer < moveToSlotDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / moveToSlotDuration);
            t = Smooth01(t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Quaternion beforeTiltRotation = transform.rotation;
        Quaternion tiltedRotation = targetRotation * Quaternion.Euler(pourRotationEuler);

        timer = 0f;

        while (timer < tiltDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / tiltDuration);
            t = Smooth01(t);

            transform.rotation = Quaternion.Slerp(beforeTiltRotation, tiltedRotation, t);

            yield return null;
        }

        transform.rotation = tiltedRotation;

        PlaySpiceSound();

        onPourMoment?.Invoke();

        yield return new WaitForSeconds(pourHoldDuration);

        StopSpiceSound();

        timer = 0f;

        while (timer < tiltDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / tiltDuration);
            t = Smooth01(t);

            transform.rotation = Quaternion.Slerp(tiltedRotation, beforeTiltRotation, t);

            yield return null;
        }

        transform.rotation = beforeTiltRotation;

        timer = 0f;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / returnDuration);
            t = Smooth01(t);

            transform.position = Vector3.Lerp(targetPosition, startPosition, t);
            transform.rotation = Quaternion.Slerp(beforeTiltRotation, startRotation, t);

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (capObject != null)
            capObject.SetActive(true);

        IsAnimating = false;
    }

    private void PlaySpiceSound()
    {
        if (spiceAudioSource == null || spicePourSound == null)
            return;

        spiceAudioSource.Stop();

        spiceAudioSource.clip = spicePourSound;
        spiceAudioSource.volume = spicePourVolume;
        spiceAudioSource.loop = false;

        spiceAudioSource.Play();
    }

    private void StopSpiceSound()
    {
        if (spiceAudioSource == null)
            return;

        spiceAudioSource.Stop();
    }

    private void OnDisable()
    {
        StopSpiceSound();

        if (capObject != null)
            capObject.SetActive(true);

        IsAnimating = false;
    }

    private float Smooth01(float value)
    {
        return value * value * (3f - 2f * value);
    }
}