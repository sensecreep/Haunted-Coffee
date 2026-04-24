using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BeanItemUI : MonoBehaviour
{
    public TextMeshProUGUI beanNameText;
    public Image[] bitternessDots;
    public Image[] acidityDots;

    private CoffeeBeans beans;
    private CoffeeGrinderUI parentUI;

    public void Setup(CoffeeBeans beans, CoffeeGrinderUI ui)
    {
        this.beans = beans;
        parentUI = ui;

        beanNameText.text = beans.beanName;

        DrawDots(bitternessDots, beans.bitterness);
        DrawDots(acidityDots, beans.acidity);

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void DrawDots(Image[] dots, int value)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].enabled = i < value;
        }
    }

    void OnClick()
    {
        parentUI.OnBeansSelected(beans);
    }
}
