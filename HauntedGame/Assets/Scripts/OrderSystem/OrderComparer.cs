using System.Linq;

public static class OrderComparer
{
    public static bool Compare(Order order, Drink drink)
    {
        if (order == null || drink == null)
            return false;

        // Тип напитка
        if (order.drinkType != drink.drinkType)
            return false;

        // Молоко
        if (order.milkAmount != drink.milkAmount)
            return false;

        // Добавки
        if (order.addons.Count != drink.addons.Count)
            return false;

        bool sameAddons = order.addons.All(a => drink.addons.Contains(a));

        if (!sameAddons)
            return false;

        //зерна
        if (drink.beans == null)
            return false;

        // ======================
        // 🌱 Предпочтение клиента
        // ======================

        bool clientWantsBitter =
            order.preferredBitterness > order.preferredAcidity;

        bool clientWantsAcidic =
            order.preferredAcidity > order.preferredBitterness;

        bool clientWantsBalanced =
            order.preferredBitterness == order.preferredAcidity;

        // ======================
        // ☕ Характер зерен
        // ======================

        bool beansAreBitter =
            drink.beans.bitterness > drink.beans.acidity;

        bool beansAreAcidic =
            drink.beans.acidity > drink.beans.bitterness;

        bool beansAreBalanced =
            drink.beans.acidity == drink.beans.bitterness;

        // ======================
        // ✅ Проверка
        // ======================

        if (clientWantsBitter && !beansAreBitter)
            return false;

        if (clientWantsAcidic && !beansAreAcidic)
            return false;

        if (clientWantsBalanced && !beansAreBalanced)
            return false;

        return true;
    }
}