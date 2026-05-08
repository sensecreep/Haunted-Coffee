using System.Collections.Generic;
using UnityEngine;
using static PitcherController;

public class DrinkAssemblyController : MonoBehaviour
{
    [Header("Links")]
    public Transform cupSlot;
    public CupController cup;           // ссылка на единственную кружку
    public PitcherController pitcher;   // ссылка на питчер

    private CupController currentCup;

    private bool isActive = false;

    private List<AddonType> addedSpices = new List<AddonType>();

    public void EnterMode()
    {
        isActive = true;
        addedSpices.Clear();

        Debug.Log("Режим сборки напитка");
    }

    public void ExitMode()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return;

        // ======================
        // ☕ ПОСТАВИТЬ КРУЖКУ
        // ======================
        if (cup.state == CupController.CupState.InHand)
        {
            TryPlaceCup();
            return;
        }

        // ======================
        // 🍯 СПЕЦИИ
        // ======================
        SpiceItem spice = hit.transform.GetComponentInParent<SpiceItem>();
        if (spice != null)
        {
            AddSpice(spice.type);
            return;
        }

        // ======================
        // 🥛 МОЛОКО
        // ======================
        if (pitcher.state == PitcherController.PitcherState.InHand)
        {
            TryAddMilk();
            return;
        }

        // ======================
        // ✅ ФИНАЛ
        // ======================
        if (hit.transform == currentCup.transform) 
        {
            FinishDrink();
        }


    }

    // ======================
    // ☕ ЧАШКА
    // ======================

    void TryPlaceCup()
    {
        if (currentCup != null)
        {
            Debug.Log("Чашка уже стоит");
            return;
        }

        if (cup == null)
        {
            Debug.Log("Нет ссылки на кружку");
            return;
        }

        if (cup.state != CupController.CupState.InHand)
        {
            Debug.Log("Кружка не в руках");
            return;
        }

        cup.PlaceInAssembly(cupSlot);
        currentCup = cup;

        Debug.Log("Чашка поставлена");
    }

    // ======================
    // 🥛 МОЛОКО
    // ======================

    void TryAddMilk()
    {

        if (pitcher == null)
        {
            Debug.Log("Нет питчера");
            return;
        }

        if (pitcher.state != PitcherController.PitcherState.InHand)
        {
            Debug.Log("Питчер не в руках");
            return;
        }

        /*if (!pitcher.isSteamed)
        {
            Debug.Log("Молоко не взбито");
            return;
        }*/

        if (pitcher.milkLevel == 0)
        {
            Debug.Log("В питчере нет молока");
            return;
        }

        if (currentCup.milkAmount > 0)
        {
            Debug.Log("Молоко уже добавлено");
            return;
        }

        currentCup.ApplyMilkFromPitcher(pitcher.milkLevel);

        pitcher.ResetToDefault();

        Debug.Log("Молоко перелито в чашку");
    }

    // ======================
    // 🍯 СПЕЦИИ
    // ======================

    void AddSpice(AddonType type)
    {
        if (currentCup == null)
        {
            Debug.Log("Нет чашки");
            return;
        }

        if (!addedSpices.Contains(type))
        {
            addedSpices.Add(type);
            currentCup.AddAddon(type);

            Debug.Log("Добавлена специя: " + type);
        }
    }

    // ======================
    // ✅ ФИНАЛ
    // ======================

    void FinishDrink()
    {
        if (currentCup == null)
        {
            Debug.Log("Нет чашки");
            return;
        }

        if (currentCup.amountOfCoffee == 0.0f)
        {
            Debug.Log("Нет кофе");
            return;
        }

        Drink drink = new Drink
        {
            milkAmount = currentCup.milkAmount,
            drinkType = currentCup.GetDrinkType(),
            addons = new List<AddonType>(currentCup.addons)
        };
        currentCup.Unlock();
        currentCup.PickUp();

        Debug.Log("Напиток собран: " + drink.drinkType);

        ResetStation();
    }

    void ResetStation()
    {
        currentCup = null;
        addedSpices.Clear();

        Debug.Log("Станция очищена");
    }
}