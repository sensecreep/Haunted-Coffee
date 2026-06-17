using UnityEngine;
using System.Collections;

public class MilkController : MonoBehaviour
{
    [Header("Links")]
    public Transform milkClickable; // коробка молока
    public PitcherController pitcher;
    public MilkStationTrigger trigger;

    [Header("Settings")]
    public int maxClicks = 2;

    [Header("FX")]
    public GameObject milkPourPrefab;
    public Transform pourPoint; // откуда льётся
    public float pourDuration = 1.5f;
    GameObject currentMilkFX;

    [Header("Milk Box Animation")]
    public Transform milkBox;          // сама пачка
    public float tiltAngle = -70f;      // угол наклона
    public float tiltDuration = 0.2f;  // скорость наклона
    public Vector3 liftOffset = new Vector3(-0.45f, -0.2f, 0.08f); // вверх + чуть вперёд
    bool isTilting = false;

    [Header("Milk Add Sound")]
    [SerializeField] private AudioSource milkAddAudioSource;
    [SerializeField] private AudioClip milkAddSound;
    [SerializeField, Range(0f, 1f)] private float milkAddVolume = 1f;

    private int currentClicks = 0;
    private bool isActive = false;

    public void EnterMilkMode()
    {
        isActive = true;
        currentClicks = 0;

        Debug.Log("Режим молока включен");
    }

    private void Start()
    {
        if (milkAddAudioSource == null)
            milkAddAudioSource = GetComponent<AudioSource>();

        if (milkAddAudioSource != null)
        {
            milkAddAudioSource.playOnAwake = false;
            milkAddAudioSource.loop = false;
        }
    }

    public void ExitMilkMode()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryPourMilk();
        }
    }

    void TryPourMilk()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return;

        if (!hit.transform.IsChildOf(milkClickable))
            return;

        AddMilk();
    }

    void AddMilk()
    {
        if (pitcher == null)
        {
            Debug.Log("Нет питчера");
            return;
        }

        //SpawnMilkFX();
        if (!isTilting)
            StartCoroutine(TiltMilkBox());

        currentClicks++;

        pitcher.AddMilkPortion();

        Debug.Log("Добавили молоко: " + currentClicks);

        if (currentClicks >= maxClicks)
        {
            Debug.Log("Молоко готово");

            //trigger.ExitMilkStation();
        }
    }
    void SpawnMilkFX()
    {
        if (milkPourPrefab == null || pourPoint == null)
            return;

        currentMilkFX = Instantiate(milkPourPrefab, pourPoint.position, pourPoint.rotation);
        //GameObject fx = Instantiate(milkPourPrefab, pourPoint.position, pourPoint.rotation);

        //Destroy(fx, pourDuration);
    }

    IEnumerator TiltMilkBox()
    {
        if (milkBox == null)
            yield break;

        isTilting = true;

        // стартовые значения
        Vector3 startPos = milkBox.localPosition;
        Quaternion startRot = milkBox.localRotation;

        // целевые
        Vector3 targetPos = startPos + liftOffset;
        Quaternion targetRot = startRot * Quaternion.Euler(-tiltAngle, 0f, 0f);

        float t = 0f;

        // 👉 движение вверх + наклон
        while (t < 1f)
        {
            t += Time.deltaTime / tiltDuration;

            milkBox.localPosition = Vector3.Lerp(startPos, targetPos, t);
            milkBox.localRotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        SpawnMilkFX();

        PlayMilkAddSound();

        // ждём пока "льётся"
        yield return new WaitForSeconds(pourDuration * 0.8f);

        // 💥 перед возвратом — убираем струю
        if (currentMilkFX != null)
        {
            Destroy(currentMilkFX);
            currentMilkFX = null;
        }

        StopMilkAddSound();

        t = 0f;

        // 👉 возврат назад
        while (t < 1f)
        {
            t += Time.deltaTime / tiltDuration;

            milkBox.localPosition = Vector3.Lerp(targetPos, startPos, t);
            milkBox.localRotation = Quaternion.Lerp(targetRot, startRot, t);

            yield return null;
        }

        // фиксируем точно
        milkBox.localPosition = startPos;
        milkBox.localRotation = startRot;

        isTilting = false;
    }

    private void PlayMilkAddSound()
    {
        if (milkAddAudioSource == null || milkAddSound == null)
            return;

        milkAddAudioSource.Stop();

        milkAddAudioSource.clip = milkAddSound;
        milkAddAudioSource.volume = milkAddVolume;
        milkAddAudioSource.loop = true;

        milkAddAudioSource.Play();
    }

    private void StopMilkAddSound()
    {
        if (milkAddAudioSource == null)
            return;

        milkAddAudioSource.Stop();
    }

    private void OnDisable()
    {
        StopMilkAddSound();
    }
}