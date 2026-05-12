using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int dayMoney = 0;

    public TextMeshProUGUI dayMoneyText;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UpdateUI();
    }
    public void AddMoney(int amount)
    {
        dayMoney += amount;
        UpdateUI();

        Debug.Log("Получено денег: " + amount + " | За день: " + dayMoney);
    }
    public int GetDayMoney()
    {
        return dayMoney;
    }
    void UpdateUI()
    {
        if (dayMoneyText != null)
            dayMoneyText.text = dayMoney + " руб.";
    }
}