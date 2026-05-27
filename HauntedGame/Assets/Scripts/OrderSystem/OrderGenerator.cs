using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class OrderGenerator
{
    public static Order GenerateRandomOrder()
    {
        DrinkType randomDrink = GetRandomDrink();

        return new Order
        {
            drinkType = randomDrink,

            milkAmount = DrinkUtils.GetMilkAmount(randomDrink),

            addons = GetRandomAddons(),

            preferredBitterness = Random.Range(0, 6),
            preferredAcidity = Random.Range(0, 6)
        };
    }

    static DrinkType GetRandomDrink()
    {
        var values = System.Enum.GetValues(typeof(DrinkType));
        return (DrinkType)values.GetValue(Random.Range(0, values.Length));
    }

    static List<AddonType> GetRandomAddons()
    {
        List<AddonType> result = new List<AddonType>();

        int count = Random.Range(0, 3); // 0–2 добавки

        var values = System.Enum.GetValues(typeof(AddonType));

        for (int i = 0; i < count; i++)
        {
            AddonType addon = (AddonType)values.GetValue(Random.Range(0, values.Length));

            if (!result.Contains(addon))
                result.Add(addon);
        }

        return result;
    }
}