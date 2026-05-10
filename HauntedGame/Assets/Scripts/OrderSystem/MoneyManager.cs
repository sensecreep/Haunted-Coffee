using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int money = 0;

    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();

        Debug.Log("Получено денег: " + amount + " | Всего: " + money);
    }

    void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = money + " руб.";
    }
}