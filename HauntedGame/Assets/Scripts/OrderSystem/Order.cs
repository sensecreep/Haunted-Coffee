using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order
{
    public DrinkType drinkType;
    public int milkAmount;        // 0-2
    public List<AddonType> addons;
    public int preferredBitterness;
    public int preferredAcidity;

    string GetBeansPreferenceText()
    {
        // случай, когда клиент вообще ничего не говорит
        if (preferredBitterness == 0 && preferredAcidity == 0)
            return "";

        if (preferredBitterness > preferredAcidity)
            return "с горчинкой";

        if (preferredAcidity > preferredBitterness)
            return "с кислинкой";

        return "сбалансированный";
    }
    public string GetFullDescription()
    {
        string result = drinkType.ToString();

        // 🌱 предпочтения
        string pref = GetBeansPreferenceText();
        if (!string.IsNullOrEmpty(pref))
        {
            result += $", {pref}";
        }

        // 🍯 добавки
        if (addons != null && addons.Count > 0)
        {
            string addonsText = string.Join(", ", addons);
            result += $", {addonsText}";
        }

        return result;
    }

    public string GetShortDescription()
    {
        string drinkText = DrinkToText(drinkType);

        string pref = GetBeansPreferenceText();

        string addonsText = (addons != null && addons.Count > 0)
            ? string.Join(", ", addons.ConvertAll(AddonToText))
            : "-";

        // если нет предпочтений
        if (string.IsNullOrEmpty(pref))
            return $"{drinkText} | {addonsText}";

        return $"{drinkText} | {pref} | {addonsText}";
    }
    public static string AddonToText(AddonType addon)
    {
        switch (addon)
        {
            case AddonType.Sugar: return "сахар";
            case AddonType.Vanilla: return "ваниль";
            case AddonType.Ginger: return "имбирь";
            case AddonType.Cinnamon: return "корица";
            default: return addon.ToString();
        }
    }
    public static string DrinkToText(DrinkType drink)
    {
        switch (drink)
        {
            case DrinkType.Espresso: return "эспрессо";
            case DrinkType.Americano: return "американо";
            case DrinkType.Latte: return "латте";
            case DrinkType.Cappuccino: return "капучино";
            default: return drink.ToString();
        }
    }
}
