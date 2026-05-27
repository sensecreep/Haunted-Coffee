using System.Linq;
using UnityEngine;

public static class OrderComparer
{
    public static OrderEvaluation Evaluate(Order order, Drink drink)
    {
        OrderEvaluation result = new OrderEvaluation();

        if (order == null || drink == null)
        {
            result.reactionType = CustomerReactionType.Bad;
            result.finalMoney = 0;
            result.mistakes.Add("Нет напитка");
            return result;
        }

        result.basePrice = GetDrinkBasePrice(order.drinkType);
        result.addonsPrice = order.addons != null ? order.addons.Count * 30 : 0;

        int fullPrice = result.basePrice + result.addonsPrice;

        if (order.drinkType != drink.drinkType)
        {
            result.reactionType = CustomerReactionType.Bad;
            result.wrongDrinkType = true;
            result.finalMoney = 0;
            result.mistakes.Add("Не тот напиток");

            return result;
        }

        int penalties = 0;

        if (order.milkAmount != drink.milkAmount)
        {
            penalties += 50;
            result.mistakes.Add("не то количество молока");
        }

        if (drink.pourQuality != PourQuality.Ideal)
        {
            penalties += 50;
            result.wrongPour = true;

            if (drink.pourQuality == PourQuality.UnderExtracted)
                result.mistakes.Add("недолив");

            if (drink.pourQuality == PourQuality.OverExtracted)
                result.mistakes.Add("перелив");
        }

        if (!BeansMatch(order, drink))
        {
            penalties += 50;
            result.wrongBeans = true;
            result.mistakes.Add("не те зерна");
        }

        int addonMistakes = CountAddonMistakes(order, drink);

        if (addonMistakes > 0)
        {
            int addonPenalty = addonMistakes * 20;

            penalties += addonPenalty;
            result.wrongAddonsCount = addonMistakes;
            result.mistakes.Add("ошибки в добавках");
        }

        result.penalties = penalties;
        result.finalMoney = Mathf.Max(0, fullPrice - penalties);

        if (penalties == 0)
            result.reactionType = CustomerReactionType.Perfect;
        else
            result.reactionType = CustomerReactionType.Normal;

        return result;
    }

    public static bool Compare(Order order, Drink drink)
    {
        return Evaluate(order, drink).reactionType == CustomerReactionType.Perfect;
    }

    static int GetDrinkBasePrice(DrinkType type)
    {
        switch (type)
        {
            case DrinkType.Espresso:
                return 150;

            case DrinkType.Americano:
                return 230;

            case DrinkType.Cappuccino:
                return 300;

            case DrinkType.Latte:
                return 350;

            default:
                return 0;
        }
    }

    static bool BeansMatch(Order order, Drink drink)
    {
        if (drink.beans == null)
            return false;

        bool clientWantsBitter =
            order.preferredBitterness > order.preferredAcidity;

        bool clientWantsAcidic =
            order.preferredAcidity > order.preferredBitterness;

        bool clientWantsBalanced =
            order.preferredBitterness == order.preferredAcidity;

        bool beansAreBitter =
            drink.beans.bitterness > drink.beans.acidity;

        bool beansAreAcidic =
            drink.beans.acidity > drink.beans.bitterness;

        bool beansAreBalanced =
            drink.beans.bitterness == drink.beans.acidity;

        if (clientWantsBitter)
            return beansAreBitter;

        if (clientWantsAcidic)
            return beansAreAcidic;

        if (clientWantsBalanced)
            return beansAreBalanced;

        return true;
    }

    static int CountAddonMistakes(Order order, Drink drink)
    {
        var orderAddons = order.addons ?? new System.Collections.Generic.List<AddonType>();
        var drinkAddons = drink.addons ?? new System.Collections.Generic.List<AddonType>();

        int mistakes = 0;

        // добавки, которые клиент просил, но их нет
        foreach (AddonType addon in orderAddons)
        {
            if (!drinkAddons.Contains(addon))
                mistakes++;
        }

        // добавки, которые игрок положил лишними
        foreach (AddonType addon in drinkAddons)
        {
            if (!orderAddons.Contains(addon))
                mistakes++;
        }

        return mistakes;
    }
}