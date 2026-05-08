using System.Linq;
using UnityEngine;

public static class OrderComparer
{
    public static bool Compare(Order order, Drink drink)
    {
        /*
        if (order.drinkType != drink.drinkType)
            return false;

        if (Mathf.Abs(order.milkAmount - drink.milkAmount) > 0.1f)
            return false;

        if (!order.addons.SequenceEqual(drink.addons))
            return false;

        return true;
        */
        return true;
    }
}
