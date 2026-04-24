using UnityEngine;

public class PlayerOrderHandler : MonoBehaviour
{
    public Order activeOrder;
    public Customer activeCustomer;

    public void TakeOrder(Customer customer)
    {
        activeCustomer = customer;
        activeOrder = customer.currentOrder;

        // показать заказ в UI
    }

    public void ServeDrink(Drink drink)
    {
        bool success = activeCustomer.CheckDrink(drink);

        if (success)
        {
            Debug.Log("Клиент доволен 😊");
        }
        else
        {
            Debug.Log("Заказ выполнен неправильно 😡");
        }

        activeOrder = null;
        activeCustomer = null;
    }
}
