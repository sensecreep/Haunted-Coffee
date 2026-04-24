using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public CoffeeBeans selectedBeans;
    public CupController currentCup;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetBeans(CoffeeBeans beans)
    {
        selectedBeans = beans;
        Debug.Log("☕ Выбраны зерна: " + beans.beanName);
    }
}
