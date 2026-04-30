using UnityEngine;
using System.Collections.Generic;

public class CupController : MonoBehaviour
{
    [Header("Drink Data")]
    public bool hasCoffee = false;
    public int milkAmount = 0; // приходит ИЗ ПИТЧЕРА
    public List<AddonType> addons = new List<AddonType>();

    public CoffeeMachineController currentMachine;
    public enum CupState
    {
        OnTable,
        InHand,
        InMachine,
        Placed
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
    public void PlaceInAssembly(Transform slot)
    {
        isLocked = true;

        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        state = CupState.Placed;

        Debug.Log("Чашка на станции сборки");
    }

    public void ApplyMilkFromPitcher(int amount)
    {
        milkAmount = amount;

        Debug.Log("Молоко применено из питчера: " + milkAmount);
    }

    public void AddCoffee()
    {
        hasCoffee = true;

        Debug.Log("В чашке есть кофе");
    }

    public void AddAddon(AddonType type)
    {
        if (!addons.Contains(type))
            addons.Add(type);

        Debug.Log("Добавка: " + type);
    }

    public DrinkType GetDrinkType()
    {
        if (!hasCoffee)
            return DrinkType.Espresso;

        switch (milkAmount)
        {
            case 0:
                return DrinkType.Espresso;

            case 1:
                return DrinkType.Cappuccino;

            case 2:
                return DrinkType.Latte;

            default:
                return DrinkType.Latte;
        }
    }
}
