using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Drink
{
    public DrinkType drinkType;
    public int milkAmount;
    public List<AddonType> addons;
    public CoffeeBeans beans;
    public PourQuality pourQuality;
}

public enum DrinkType
{
    Espresso,
    Americano,
    Latte,
    Cappuccino
}

public enum PourQuality
{
    UnderExtracted, // недолив
    Ideal,
    OverExtracted  // перелив
}

public enum AddonType
{
    Sugar,
    Vanilla,
    Ginger,
    Cinnamon
}

public static class DrinkUtils
{
    public static int GetMilkAmount(DrinkType type)
    {
        switch (type)
        {
            case DrinkType.Latte:
                return 2;

            case DrinkType.Cappuccino:
                return 1;

            case DrinkType.Espresso:
            case DrinkType.Americano:
                return 0;

            default:
                return 0;
        }
    }
}