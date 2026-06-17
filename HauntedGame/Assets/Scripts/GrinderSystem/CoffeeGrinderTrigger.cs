using System.Collections;
using UnityEngine;
using static PortafilterController;

public class CoffeeGrinderTrigger : MonoBehaviour
{
    public GameObject pressEUI;
    public CoffeeGrinderUI grinderUI;
    public Transform cameraPoint;
    public Transform playerSpot;
    public Transform portafilterSlot;
    public PortafilterController portafilter;
    //public Transform grinderClickable; // сама модель кофемолки

    [Header("Grinder Sound")]
    public AudioSource grinderAudioSource;
    public AudioClip grindingSound;
    public float grindingDuration = 2f;

    private bool playerInRange;
    private bool isUsing;
    private bool isGrinding;

    void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (grinderUI != null)
            grinderUI.gameObject.SetActive(false);

        if (grinderAudioSource == null)
            grinderAudioSource = GetComponent<AudioSource>();

        if (grinderAudioSource != null)
        {
            grinderAudioSource.playOnAwake = false;
            grinderAudioSource.loop = false;
        }
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenGrinder();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseGrinder();
        }
    }

    void OpenGrinder()
    {
        if (isGrinding)
            return;

        isUsing = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        MovePlayerToSpot();

        PlayerLock.Instance.Lock();

        CameraFocusController.Instance.FocusOn(cameraPoint);

        if (grinderUI != null)
        {
            grinderUI.gameObject.SetActive(true);
            grinderUI.Open(this);
        }
    }

    void MovePlayerToSpot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = playerSpot.position;
        player.transform.rotation = playerSpot.rotation;
    }
    public bool TryGrindSelectedBeans(CoffeeBeans beans)
    {
        if (isGrinding)
        {
            Debug.Log("Кофемолка уже работает");
            return false;
        }

        if (beans == null)
        {
            Debug.LogWarning("Не выбраны зерна");
            return false;
        }

        if (portafilter == null)
        {
            Debug.LogError("Холдер не назначен в CoffeeGrinderTrigger");
            return false;
        }

        if (portafilter.state != PortafilterController.PortafilterState.InGrinder)
        {
            Debug.Log("Сначала вставьте холдер в кофемолку");
            return false;
        }

        StartCoroutine(GrindingRoutine(beans));
        //portafilter.FillWithGroundCoffee(beans);

        Debug.Log("Помол выполнен. Зерна в холдере: " + beans.beanName);
        return true;
    }

    private IEnumerator GrindingRoutine(CoffeeBeans beans)
    {
        isGrinding = true;

        if (portafilter != null)
            portafilter.isLocked = true;

        if (grinderUI != null)
            grinderUI.gameObject.SetActive(false);

        PlayGrindingSound();

        yield return new WaitForSeconds(grindingDuration);

        if (portafilter != null)
        {
            portafilter.FillWithGroundCoffee(beans);
            portafilter.isLocked = false;
        }

        isGrinding = false;

        Debug.Log("Помол выполнен. Зерна в холдере: " + beans.beanName);
    }

    private void PlayGrindingSound()
    {
        if (grinderAudioSource == null)
            return;

        grinderAudioSource.Stop();

        if (grindingSound != null)
            grinderAudioSource.clip = grindingSound;

        grinderAudioSource.Play();
    }

    public void CloseGrinder()
    {
        if (isGrinding)
        {
            Debug.Log("Нельзя закрыть кофемолку во время помола");
            return;
        }

        isUsing = false;

        grinderUI.gameObject.SetActive(false);

        CameraFocusController.Instance.Return();

        PlayerLock.Instance.Unlock();
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (pressEUI != null)
            pressEUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (pressEUI != null)
            pressEUI.SetActive(false);
    }
    public bool IsUsing()
    {
        return isUsing;
    }
    public bool IsGrinding()
    {
        return isGrinding;
    }
}
