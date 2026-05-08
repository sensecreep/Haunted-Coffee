using UnityEngine;
using UnityEngine.UI;
using static PortafilterController;
using UnityEngine.EventSystems;

public class CoffeeMachineController : MonoBehaviour
{
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

    void Start()
    {
        machineUI.SetActive(false);
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

        portafilter.InsertIntoGrinder(portafilterSlot);
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

        Debug.Log("Начался пролив");
    }

    void ContinuePour()
    {
        currentValue += Time.deltaTime * pourSpeed;
        pourSlider.value = currentValue;
    }

    void StopPour()
    {
        isHoldingButton = false;

        buttonRenderer.material = readyMat;

        currentCup.AddCoffee(currentValue);

        Debug.Log("Остановлено: " + currentValue);

        Evaluate();

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

    void Evaluate()
    {
        if (currentValue < idealMin)
            Debug.Log("Недолив ❌");
        else if (currentValue > idealMax)
            Debug.Log("Перелив ❌");
        else
            Debug.Log("Идеально ☕✅");
    }
}