using UnityEngine;

public class CupController : MonoBehaviour
{
    public CoffeeMachineController currentMachine;
    public enum CupState
    {
        OnTable,
        InHand,
        InMachine
    }

    public CupState state = CupState.OnTable;

    public Transform handPoint;

    private bool isLocked = false;

    public void PickUp()
    {
        PlayerInventory.Instance.currentCup = this;

        if (isLocked) return;

        state = CupState.InHand;

        transform.SetParent(handPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log("Чашка в руках");

        // 🔥 ВАЖНО — если забрали из машины
        if (currentMachine != null)
        {
            currentMachine.OnCupTaken();
            currentMachine = null;
        }
    }

    public void PlaceInMachine(Transform slot)
    {
        state = CupState.InMachine;

        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        PlayerInventory.Instance.currentCup = null; // 🔥 очищаем

        isLocked = true;

        Debug.Log("Чашка поставлена в кофемашину");
    }

    public void Unlock()
    {
        isLocked = false;
    }

    void OnMouseDown()
    {
        if (state == CupState.OnTable)
        {
            PickUp();
        }

        if (state == CupState.InMachine && !isLocked)
        {
            PickUp();
        }
    }
}
