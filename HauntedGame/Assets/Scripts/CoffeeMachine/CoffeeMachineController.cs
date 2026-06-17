using UnityEngine;
using UnityEngine.UI;
using static PortafilterController;
using UnityEngine.EventSystems;

public class CoffeeMachineController : MonoBehaviour
{
    public Image pourFillImage;

    [Header("Coffee Machine Sound")]
    public AudioSource pourAudioSource;
    public AudioClip pourSound;
    [Range(0f, 1f)] public float pourSoundVolume = 1f;

    [Header("Progress Zones")]
    public RectTransform zonesRoot;
    public Image underZoneImage;
    public Image idealZoneImage;
    public Image overZoneImage;

    public Color underColor = new Color(1f, 0.2f, 0.2f, 0.6f);
    public Color idealColor = new Color(0.2f, 1f, 0.3f, 0.7f);
    public Color overColor = new Color(1f, 0.2f, 0.2f, 0.6f);

    [Header("Links")]
    public Transform portafilterSlot;
    public PortafilterController portafilter;
    public CoffeeMachineTrigger trigger;

    [Header("Cup")]
    public Transform cupSlot;
    public CupController currentCup;
    public bool hasCup = false;

    [Header("Button")]
    public Renderer buttonRenderer;
    public Material inactiveMat;
    public Material readyMat;      // зелёный
    public Material pressedMat;    // тёмно-зелёный

    [Header("UI")]
    public GameObject machineUI;
    public Slider pourSlider;

    [Header("Settings")]
    public float pourSpeed = 0.25f;
    public float idealMin = 0.45f;
    public float idealMax = 0.55f;

    bool isHoldingButton = false;
    private bool isActive = false;
    private float currentValue = 0f;

    private State state = State.Idle;

    enum State
    {
        Idle,
        HasPortafilter,
        Pouring,
        Finished
    }

    void SetupPourZones()
    {
        if (underZoneImage == null || idealZoneImage == null || overZoneImage == null)
            return;

        underZoneImage.color = underColor;
        idealZoneImage.color = idealColor;
        overZoneImage.color = overColor;

        SetZone(underZoneImage.rectTransform, 0f, idealMin);
        SetZone(idealZoneImage.rectTransform, idealMin, idealMax);
        SetZone(overZoneImage.rectTransform, idealMax, 1f);
    }

    void SetZone(RectTransform rect, float min, float max)
    {
        rect.anchorMin = new Vector2(min, 0f);
        rect.anchorMax = new Vector2(max, 1f);

        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void Start()
    {
        machineUI.SetActive(false);

        pourSlider.minValue = 0f;
        pourSlider.maxValue = 1f;
        pourSlider.value = 0f;

        SetupPourZones();

        if (pourAudioSource == null)
            pourAudioSource = GetComponent<AudioSource>();

        if (pourAudioSource != null)
        {
            pourAudioSource.playOnAwake = false;
            pourAudioSource.loop = true;
            pourAudioSource.volume = pourSoundVolume;

            if (pourSound != null)
                pourAudioSource.clip = pourSound;
        }
    }
    public void EnterMachineMode()
    {
        isActive = true;
        machineUI.SetActive(true);
        state = State.Idle;
    }

    public void ExitMachineMode()
    {
        isActive = false;
        isHoldingButton = false;

        StopPourSound();

        machineUI.SetActive(false);

        CameraFocusController.Instance.Return();
        PlayerLock.Instance.Unlock();
    }

    void Update()
    {
        if (!isActive) return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 🖱 НАЖАЛ
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();

            // если можно лить и клик был по кнопке
            if (state == State.HasPortafilter && hasCup && IsPointerOnButton())
            {
                isHoldingButton = true;
                StartPour();
            }
        }

        // 🔄 УДЕРЖАНИЕ
        if (isHoldingButton && Input.GetMouseButton(0))
        {
            ContinuePour();
        }

        // ✋ ОТПУСТИЛ
        if (isHoldingButton && Input.GetMouseButtonUp(0))
        {
            isHoldingButton = false;
            StopPour();
        }

        /*
        if (state == State.Pouring)
        {
            currentValue += Time.deltaTime * pourSpeed;
            pourSlider.value = currentValue;
        } */
    }

    void UpdateButtonVisual()
    {
        if (state == State.HasPortafilter && hasCup)
        {
            buttonRenderer.material = readyMat; // ✅ готова (зелёная)
        }
        else
        {
            buttonRenderer.material = inactiveMat; // ❌ неактивна
        }
    }

    /*
    void UpdateButtonState()
    {
        if (state == State.HasPortafilter && hasCup)
        {
            buttonRenderer.material = activeMat;
        }
        else
        {
            buttonRenderer.material = inactiveMat;
        }
    } */

    void HandleClick()
    {
        Debug.Log("STATE = " + state);

        //if (!IsClickOnMachine()) return;

        switch (state)
        {
            case State.Idle:
                TryInsert();
                break;

            case State.HasPortafilter:
                TryInsertCup();

                /*
                // если уже есть чашка → запускаем пролив
                if (hasCup)
                {
                    StartPour();
                } */
                break;

            case State.Pouring:
                StopPour();
                break;
        }
    }

    /*
    bool IsClickOnMachine()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return false;

        return hit.transform == buttonRenderer.transform;
    }
    */

    void TryInsert()
    {
        Debug.Log("TRY INSERT ВЫЗВАН");
        if (portafilter.state != PortafilterState.InHand)
            return;

        if (!portafilter.hasCoffee || portafilter.BeansInPortafilter == null)
        {
            Debug.Log("В холдере нет молотого кофе");
            return;
        }

        Camera cam = CameraFocusController.Instance.GetActiveCamera();

        Debug.Log(cam.name);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("Попали хоть куда-то: " + hit.transform.name);
        }
        else
        {
            Debug.Log("Вообще никуда не попали");
        }
        //if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            //return;

        if (!hit.transform.IsChildOf(transform))
            return;

        Debug.Log("Попали в: " + hit.transform.name);

        portafilter.InsertIntoMachine(portafilterSlot);
        portafilter.isLocked = true; // 🔥 блокируем

        state = State.HasPortafilter;

        UpdateButtonVisual();
        //UpdateButtonState();

        Debug.Log("Холдер вставлен");
    }

    void TryInsertCup()
    {
        Debug.Log("TRY INSERT CUP");

        if (currentCup != null)
        {
            Debug.Log("Уже есть чашка в машине");
            return;
        }

        CupController cup = PlayerInventory.Instance.currentCup;

        if (cup == null)
        {
            Debug.Log("У игрока нет чашки");
            return;
        }

        if (cup.state != CupController.CupState.InHand)
        {
            Debug.Log("Чашка не в руках");
            return;
        }

        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return;
  
        if (!hit.transform.GetComponentInParent<CoffeeMachineController>())
            return;

        cup.PlaceInMachine(cupSlot);

        cup.currentMachine = this;

        currentCup = cup;
        hasCup = true;

        UpdateButtonVisual();
        //UpdateButtonState();

        Debug.Log("Чашка вставлена");
    }

    public void OnCupTaken()
    {
        Debug.Log("Чашка забрана — выходим из машины");

        trigger.CloseMachine();

        ResetMachine();
    }

    void ResetMachine()
    {
        currentValue = 0f;
        pourSlider.value = 0f;

        if (pourFillImage != null)
            pourFillImage.color = Color.red;

        hasCup = false;
        currentCup = null;

        state = State.Idle;

        UpdateButtonVisual();
    }

    void StartPour()
    {
        state = State.Pouring;
        currentValue = 0f;
        portafilter.isLocked = true; // на всякий
        buttonRenderer.material = pressedMat; // 🔥 тёмно-зелёный

        PlayPourSound();

        Debug.Log("Начался пролив");
    }

    void ContinuePour()
    {
        currentValue += Time.deltaTime * pourSpeed;
        currentValue = Mathf.Clamp01(currentValue);

        pourSlider.value = currentValue;

        UpdatePourFillColor();

        if (currentValue >= 1f)
        {
            StopPour();
        }
    }

    void UpdatePourFillColor()
    {
        if (pourFillImage == null)
            return;

        if (currentValue >= idealMin && currentValue <= idealMax)
        {
            pourFillImage.color = Color.green;
        }
        else
        {
            pourFillImage.color = Color.red;
        }
    }

    void StopPour()
    {
        StopPourSound();

        isHoldingButton = false;

        buttonRenderer.material = readyMat;

        CoffeeBeans extractedBeans = portafilter.BeansInPortafilter;
        currentCup.AddCoffee(currentValue, extractedBeans);

        PourQuality quality = Evaluate();
        currentCup.pourQuality = quality;

        Debug.Log("Остановлено: " + currentValue + " | Пролив: " + quality);

        // 🔥 сброс холдера
        portafilter.ResetToDefault();

        // 🔥 разблокируем чашку
        if (currentCup != null)
        {
            currentCup.Unlock();
        }

        // 🔥 сбрасываем состояние машины
        state = State.Idle;
        hasCup = false;
        currentCup = null;

        UpdateButtonVisual();
    }

    bool IsPointerOnButton()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return false;

        return hit.transform == buttonRenderer.transform;
    }

    PourQuality Evaluate()
    {
        if (currentValue < idealMin)
        {
            Debug.Log("Недолив ❌");
            return PourQuality.UnderExtracted;
        }

        if (currentValue > idealMax)
        {
            Debug.Log("Перелив ❌");
            return PourQuality.OverExtracted;
        }

        Debug.Log("Идеально ☕✅");
        return PourQuality.Ideal;
    }

    private void PlayPourSound()
    {
        if (pourAudioSource == null)
            return;

        if (pourSound != null)
            pourAudioSource.clip = pourSound;

        pourAudioSource.volume = pourSoundVolume;
        pourAudioSource.loop = true;

        if (!pourAudioSource.isPlaying)
            pourAudioSource.Play();
    }

    private void StopPourSound()
    {
        if (pourAudioSource == null)
            return;

        pourAudioSource.Stop();
    }

    private void OnDisable()
    {
        StopPourSound();
    }
}