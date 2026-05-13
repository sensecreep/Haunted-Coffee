using UnityEngine;

public class CoffeeGrinderUI : MonoBehaviour
{
    public CoffeeBeansData beansDatabase;
    public BeanItemUI beanItemPrefab;
    public Transform beansParent;

    private CoffeeGrinderTrigger trigger;

    public void Open(CoffeeGrinderTrigger grinderTrigger)
    {
        trigger = grinderTrigger;
        BuildList();
    }

    void BuildList()
    {
        if (beansDatabase == null)
        {
            Debug.LogError("BeansDatabase не назначен");
            return;
        }

        foreach (Transform child in beansParent)
            Destroy(child.gameObject);

        float startY = 300f;
        float spacing = -218f; // расстояние между карточками

        foreach (var bean in beansDatabase.beans)
        {
            var item = Instantiate(beanItemPrefab, beansParent);
            var rect = item.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(0, startY);
            startY += spacing;
            
            item.Setup(bean, this);
        }
    }

    public void OnBeansSelected(CoffeeBeans beans)
    {
        PlayerInventory.Instance.SetBeans(beans);
        gameObject.SetActive(false);

        //trigger.CloseGrinder();
    }
}
