using UnityEngine;

public class PitcherController : MonoBehaviour
{
    public enum PitcherState
    {
        Idle,
        InHand
    }

    [Header("State")]
    public PitcherState state = PitcherState.Idle;

    [Header("Milk")]
    public int milkLevel = 0;

    [Header("Links")]
    public Transform handPoint;
    public MilkStationTrigger currentStation;

    [Header("Settings")]
    public bool isLocked = false;
    public bool isSteamed = false;

    void Update()
    {
        // простой клик по питчеру
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return;

        if (hit.transform != transform)
            return;

        switch (state)
        {
            case PitcherState.Idle:
                PickUp();
                break;

            case PitcherState.InHand:
                // позже можно добавить "поставить"
                break;
        }
    }

    public void PickUp()
    {
        if (isLocked) return;

        state = PitcherState.InHand;

        transform.SetParent(handPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log("Питчер в руках");

        // 👉 записываем в инвентарь (если используешь)
        //PlayerInventory.Instance.currentPitcher = this;

        // 🔥 ВАЖНО — выход из станции молока
        if (currentStation != null)
        {
            currentStation.ExitMilkStation();
            currentStation = null;
        }
    }

    public void AddMilkPortion()
    {
        milkLevel++;

        Debug.Log("Молоко в питчере: " + milkLevel);
    }

    public void ResetMilk()
    {
        milkLevel = 0;
    }

    public void InsertToSteamWand(Transform slot)
    {
        isLocked = true;

        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        state = PitcherState.Idle;

        Debug.Log("Питчер под паровой трубкой");
    }
    public void ReturnToHand()
    {
        isLocked = false;

        state = PitcherState.InHand;

        transform.SetParent(handPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log("Питчер вернулся в руки");
    }
    public void SetSteamed(bool value)
    {
        isSteamed = value;

        Debug.Log("Молоко взбито: " + value);

        // 👉 тут потом можно сменить материал
    }
}