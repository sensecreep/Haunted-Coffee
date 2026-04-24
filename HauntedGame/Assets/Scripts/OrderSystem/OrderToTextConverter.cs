using System.Collections.Generic;
using UnityEngine;

public static class OrderToTextConverter
{
    public static string Convert(Order order)
    {
        string text = $"Мне, пожалуйста, {DrinkToText(order.drinkType)}";

        // 🌱 предпочтения по зернам
        string pref = GetBeansPreferenceText(order);
        if (!string.IsNullOrEmpty(pref))
        {
            text += $", {pref}";
        }

        // 🍯 добавки
        if (order.addons != null && order.addons.Count > 0)
        {
            text += "\nДобавки: ";

            for (int i = 0; i < order.addons.Count; i++)
            {
                text += AddonToText(order.addons[i]);

                if (i < order.addons.Count - 1)
                    text += ", ";
            }
        }

        return text;
    }

    static string GetBeansPreferenceText(Order order)
    {
        if (order.preferredBitterness == 0 && order.preferredAcidity == 0)
            return "";

        if (order.preferredBitterness > order.preferredAcidity)
            return RandomPhrase(new[] { "покрепче", "погорче" });

        if (order.preferredAcidity > order.preferredBitterness)
            return RandomPhrase(new[] { "с кислинкой", "покислее" });

        return "сбалансированный";
    }
    static string RandomPhrase(string[] variants)
    {
        return variants[UnityEngine.Random.Range(0, variants.Length)];
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
