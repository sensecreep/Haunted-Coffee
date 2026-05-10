using UnityEngine;

public class Customer : MonoBehaviour
{
    public Order currentOrder;

    private string[] orderDialogueLines;

    public bool isCafeCustomer = true;

    public CustomerState State { get; private set; } = CustomerState.Idle;

    private void Start()
    {
        GenerateOrder();
        BuildDialogueLines();
    }

    public OrderEvaluation EvaluateDrink(Drink drink)
    {
        return OrderComparer.Evaluate(currentOrder, drink);
    }

    public void Serve()
    {
        State = CustomerState.Served;
    }

    void GenerateOrder()
    {
        currentOrder = OrderGenerator.GenerateRandomOrder();
    }

    void BuildDialogueLines()
    {
        orderDialogueLines = new string[]
        {
            "Здравствуйте!",
            OrderToTextConverter.Convert(currentOrder)
        };
    }

    public string[] GetOrderDialogue()
    {
        return orderDialogueLines;
    }

    // НОВОЕ: вызываем после окончания диалога
    public void AcceptOrder()
    {
        if (State != CustomerState.Idle)
            return;

        State = CustomerState.WaitingForDrink;
    }

    public bool CanStartDialogue()
    {
        return State == CustomerState.Idle;
    }

    public bool CheckDrink(Drink drink)
    {
        return OrderComparer.Compare(currentOrder, drink);
    }

    public enum CustomerState
    {
        Idle,
        OrderTaken,
        WaitingForDrink,
        Served
    }
}
