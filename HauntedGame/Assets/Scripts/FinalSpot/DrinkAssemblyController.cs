using UnityEngine;
using System.Collections.Generic;

public class DrinkAssemblyController : MonoBehaviour
{
    [Header("Slots")]
    public Transform cupSlot;

    [Header("Links")]
    public CupController currentCup;
    public PitcherController pitcher;

    private bool isActive;

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

        // 👉 чашка
        if (hit.transform.GetComponentInParent<CupController>())
        {
            TryPlaceCup();
            return;
        }

        // 👉 специи
        SpiceItem spice = hit.transform.GetComponentInParent<SpiceItem>();
        if (spice != null)
        {
            AddSpice(spice.type);
            return;
        }

        // 👉 условный “финал”
        if (hit.transform.CompareTag("FinishDrink"))
        {
            FinishDrink();
        }
    }

    void TryPlaceCup()
    {
        CupController cup = currentCup;

        if (cup == null) return;

        cup.PlaceInAssembly(cupSlot);

        currentCup = cup;

        Debug.Log("Чашка поставлена");
    }

    void AddSpice(AddonType type)
    {
        addedSpices.Add(type);

        Debug.Log("Добавлена специя: " + type);
    }

    void FinishDrink()
    {
        if (currentCup == null)
        {
            Debug.Log("Нет чашки");
            return;
        }

        Drink drink = new Drink
        {
            drinkType = currentCup.GetDrinkType(),
            addons = new List<AddonType>(currentCup.addons),
            milkAmount = currentCup.milkAmount
        };

        Debug.Log("Напиток: " + drink.drinkType + " | молоко: " + drink.milkAmount);
    }
}