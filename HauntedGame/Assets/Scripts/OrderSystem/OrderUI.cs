using UnityEngine;
using TMPro;

public class OrderUI : MonoBehaviour
{
    public static OrderUI Instance;

    public GameObject panel;
    public TextMeshProUGUI orderText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowOrder(Order order)
    {
        panel.SetActive(true);
        orderText.text = order.GetShortDescription();
    }

    public void Clear()
    {
        panel.SetActive(false);
        orderText.text = "";
    }
}
