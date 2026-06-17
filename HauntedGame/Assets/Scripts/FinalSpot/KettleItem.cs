using System.Collections;
using UnityEngine;

public class KettleItem : MonoBehaviour
{
    [Header("State")]
    public bool hasHotWater = true;

    [Header("References")]
    [SerializeField] private Transform kettle;
    [SerializeField] private Transform kettlePourSlot;
    [SerializeField] private Transform pourPoint;
    [SerializeField] private Transform cupTarget;
    [SerializeField] private LineRenderer waterStream;
    [SerializeField] private GameObject waterVisualInCup;

    [Header("Pour Settings")]
    [SerializeField] private float pourDuration = 1.5f;
    [SerializeField] private float returnDuration = 0.5f;
    [SerializeField] private float tiltAngle = 55f;

    [Header("Water Pour Sound")]
    [SerializeField] private AudioSource waterPourAudioSource;
    [SerializeField] private AudioClip waterPourSound;
    [SerializeField, Range(0f, 1f)] private float waterPourVolume = 1f;

    private Transform startParent;
    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;
    private bool startTransformCached = false;

    private bool isPouring = false;
    public bool IsPouring => isPouring;

    private void Start()
    {
        if (kettle == null)
            kettle = transform;

        CacheStartTransform();

        if (waterStream != null)
        {
            waterStream.positionCount = 2;
            waterStream.useWorldSpace = true;
            waterStream.gameObject.SetActive(false);
        }

        if (waterVisualInCup != null)
            waterVisualInCup.SetActive(false);

        if (waterPourAudioSource == null)
            waterPourAudioSource = GetComponent<AudioSource>();

        if (waterPourAudioSource != null)
        {
            waterPourAudioSource.playOnAwake = false;
            waterPourAudioSource.loop = true;
            waterPourAudioSource.volume = waterPourVolume;

            if (waterPourSound != null)
                waterPourAudioSource.clip = waterPourSound;
        }
    }

    public bool StartPourWater()
    {
        if (isPouring)
            return false;

        if (kettle == null)
            kettle = transform;

        if (kettlePourSlot == null)
        {
            Debug.LogError("KettlePourSlot не назначен в KettleItem");
            return false;
        }

        if (pourPoint == null)
        {
            Debug.LogError("PourPoint не назначен в KettleItem");
            return false;
        }

        if (cupTarget == null)
        {
            Debug.LogError("CupTarget не назначен в KettleItem");
            return false;
        }

        if (waterStream == null)
        {
            Debug.LogError("WaterStream LineRenderer не назначен в KettleItem");
            return false;
        }

        StartCoroutine(PourWaterRoutine());
        return true;
    }

    private IEnumerator PourWaterRoutine()
    {
        isPouring = true;

        if (!startTransformCached)
            CacheStartTransform();

        kettle.SetParent(kettlePourSlot);
        kettle.localPosition = Vector3.zero;
        kettle.localRotation = Quaternion.identity;

        Quaternion pourStartRotation = kettle.localRotation;
        Quaternion targetRotation = pourStartRotation * Quaternion.Euler(0f, 0f, tiltAngle);

        waterStream.gameObject.SetActive(true);
        PlayWaterPourSound();

        float timer = 0f;

        while (timer < pourDuration)
        {
            timer += Time.deltaTime;
            float t = timer / pourDuration;

            kettle.localRotation = Quaternion.Lerp(pourStartRotation, targetRotation, t);
            UpdateWaterStream();

            yield return null;
        }

        if (waterVisualInCup != null)
            waterVisualInCup.SetActive(true);

        timer = 0f;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float t = timer / returnDuration;

            kettle.localRotation = Quaternion.Lerp(targetRotation, pourStartRotation, t);
            UpdateWaterStream();

            yield return null;
        }

        kettle.localRotation = pourStartRotation;
        waterStream.gameObject.SetActive(false);
        StopWaterPourSound();

        ReturnKettleToStartPoint();

        isPouring = false;

        Debug.Log("Вода перелита в чашку");
    }

    private void UpdateWaterStream()
    {
        if (waterStream == null || pourPoint == null || cupTarget == null)
            return;

        waterStream.SetPosition(0, pourPoint.position);
        waterStream.SetPosition(1, cupTarget.position);
    }

    private void PlayWaterPourSound()
    {
        if (waterPourAudioSource == null || waterPourSound == null)
            return;

        waterPourAudioSource.Stop();

        waterPourAudioSource.clip = waterPourSound;
        waterPourAudioSource.volume = waterPourVolume;
        waterPourAudioSource.loop = true;

        waterPourAudioSource.Play();
    }
    private void StopWaterPourSound()
    {
        if (waterPourAudioSource == null)
            return;

        waterPourAudioSource.Stop();
    }
    private void CacheStartTransform()
    {
        if (kettle == null)
            return;

        startParent = kettle.parent;
        startLocalPosition = kettle.localPosition;
        startLocalRotation = kettle.localRotation;
        startTransformCached = true;
    }

    private void ReturnKettleToStartPoint()
    {
        if (kettle == null)
            return;

        kettle.SetParent(startParent);
        kettle.localPosition = startLocalPosition;
        kettle.localRotation = startLocalRotation;
    }
    private void OnDisable()
    {
        StopWaterPourSound();

        if (waterStream != null)
            waterStream.gameObject.SetActive(false);

        isPouring = false;
    }
}
