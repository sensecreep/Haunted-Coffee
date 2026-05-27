using System.Collections.Generic;
using UnityEngine;
using static PitcherController;

public class DrinkAssemblyController : MonoBehaviour
{
    [SerializeField] private MilkPourController milkPourController;

    [Header("Water")]
    public KettleItem kettle;

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

        if (cup.state == CupController.CupState.InHand)
        {
            TryPlaceCup();
            return;
        }

        SpiceItem spice = hit.transform.GetComponentInParent<SpiceItem>();
        if (spice != null)
        {
            AddSpice(spice.type);
            return;
        }

        if (pitcher.state == PitcherController.PitcherState.InHand)
        {
            TryAddMilk();
            return;
        }

        KettleItem clickedKettle = hit.transform.GetComponentInParent<KettleItem>();
        if (clickedKettle != null)
        {
            TryAddWater(clickedKettle);
            return;
        }

        if (hit.transform == currentCup.transform)
        {
            FinishDrink();
        }
    }

    void TryAddWater(KettleItem clickedKettle)
    {
        if (currentCup == null)
        {
            Debug.Log("Нет чашки");
            return;
        }

        if (currentCup.amountOfCoffee == 0.0f)
        {
            Debug.Log("Сначала нужно добавить кофе");
            return;
        }

        if (currentCup.milkAmount > 0)
        {
            Debug.Log("Нельзя добавить воду в молочный напиток");
            return;
        }

        if (currentCup.hasWater)
        {
            Debug.Log("Кипяток уже добавлен");
            return;
        }

        if (!clickedKettle.hasHotWater)
        {
            Debug.Log("В чайнике нет кипятка");
            return;
        }

        if (clickedKettle.IsPouring)
        {
            Debug.Log("Вода уже наливается");
            return;
        }

        bool animationStarted = clickedKettle.StartPourWater();

        if (!animationStarted)
            return;

        currentCup.AddWater();

        Debug.Log("Добавлен кипяток — получится американо");
    }

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

        milkPourController.StartPourMilk();
        currentCup.ApplyMilkFromPitcher(pitcher.milkLevel);

        //pitcher.ResetToDefault();

        Debug.Log("Молоко перелито в чашку");
    }

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
            beans = currentCup.beans,
            milkAmount = currentCup.milkAmount,
            drinkType = currentCup.GetDrinkType(),
            addons = new List<AddonType>(currentCup.addons),
            pourQuality = currentCup.pourQuality,
            hasWater = currentCup.hasWater
        };

        PlayerInventory.Instance.currentDrink = drink;

        currentCup.Unlock();
        currentCup.PickUp();

        Debug.Log("Напиток собран: " + drink.drinkType);
        Debug.Log("Напиток собран: " + drink.beans);
        Debug.Log("Напиток собран: " + drink.milkAmount);
        Debug.Log("Напиток собран: " + drink.addons);
        Debug.Log("Напиток собран: " + drink.pourQuality);
        Debug.Log("Напиток собран: " + drink.hasWater);

        ResetStation();
    }

    void ResetStation()
    {
        currentCup = null;
        addedSpices.Clear();

        Debug.Log("Станция очищена");
    }
}