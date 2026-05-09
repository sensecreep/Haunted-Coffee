using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public string dialogueId = "cafe_npc";
    public DialogeBox dialogueBox;
    public GameObject pressEUI;

    private NPCController npcController;
    private Customer customer;

    private bool dialogueStarted;

    public Transform npcLookPoint;
    private CameraDialogueController cameraController;

    private MonoBehaviour playerController;

    private DialogueMode currentDialogueMode = DialogueMode.None;

    enum DialogueMode
    {
        None,
        Order,
        ServeReaction
    }

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        cameraController = Camera.main.GetComponent<CameraDialogueController>();

        pressEUI.SetActive(false);
        dialogueBox.gameObject.SetActive(false);

        npcController = GetComponent<NPCController>();
        customer = GetComponent<Customer>();
    }

    void Update()
    {
        // Игрок далеко
        if (!npcController.PlayerIsInTalkDistance)
        {
            pressEUI.SetActive(false);
            dialogueStarted = false;
            return;
        }

        // Если диалог открыт
        if (dialogueBox.gameObject.activeSelf)
        {
            pressEUI.SetActive(false);
            return;
        }

        if (customer != null && customer.State == Customer.CustomerState.Served)
        {
            pressEUI.SetActive(false);
            return;
        }

        pressEUI.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (customer != null &&
                customer.State == Customer.CustomerState.WaitingForDrink)
            {
                TryServeDrink();
                return;
            }

            if (customer != null &&
                customer.State == Customer.CustomerState.Idle)
            {
                StartOrderDialogue();
                return;
            }
        }
    }

    void StartOrderDialogue()
    {
        playerController.enabled = false;

        cameraController.StartDialogueLook(npcLookPoint);

        dialogueStarted = true;
        currentDialogueMode = DialogueMode.Order;

        pressEUI.SetActive(false);

        string[] lines = customer != null
            ? customer.GetOrderDialogue()
            : DialogueManager.Instance.GetDialogue(dialogueId);

        dialogueBox.gameObject.SetActive(true);
        dialogueBox.StartDialogue(lines);
    }
    private void OnEnable()
    {
        DialogeBox.OnDialogueEnded += HandleDialogueEnded;
    }

    private void OnDisable()
    {
        DialogeBox.OnDialogueEnded -= HandleDialogueEnded;
    }

    void HandleDialogueEnded()
    {
        if (!dialogueStarted)
            return;

        playerController.enabled = true;
        dialogueStarted = false;
        cameraController.EndDialogueLook();

        if (currentDialogueMode == DialogueMode.Order)
        {
            if (customer != null)
            {
                customer.AcceptOrder();
                OrderUI.Instance.ShowOrder(customer.currentOrder);
            }
        }
        else if (currentDialogueMode == DialogueMode.ServeReaction)
        {
            npcController.LeaveCafe(() =>
            {
                CustomerSpawner.Instance.SpawnNextCustomer();
            });
        }

        currentDialogueMode = DialogueMode.None;
    }

    void TryServeDrink()
    {
        Drink playerDrink = PlayerInventory.Instance.currentDrink;

        if (playerDrink == null)
        {
            Debug.Log("У игрока нет напитка");
            return;
        }

        bool success = customer.CheckDrink(playerDrink);

        ResetPlayerDrinkAndCup();

        customer.Serve();

        playerController.enabled = false;
        cameraController.StartDialogueLook(npcLookPoint);

        dialogueStarted = true;
        currentDialogueMode = DialogueMode.ServeReaction;

        pressEUI.SetActive(false);

        if (success)
        {
            Debug.Log("Клиент доволен 😊");

            dialogueBox.gameObject.SetActive(true);
            dialogueBox.StartDialogue(new string[]
            {
            "Спасибо!",
            "Именно то, что я заказывал."
            });
        }
        else
        {
            Debug.Log("Заказ неверный 😡");

            dialogueBox.gameObject.SetActive(true);
            dialogueBox.StartDialogue(new string[]
            {
            "Это не мой заказ.",
            "Я просил другой напиток."
            });
        }

        //customer.Serve();

        //PlayerInventory.Instance.currentDrink = null;
    }

    void ResetPlayerDrinkAndCup()
    {
        PlayerInventory.Instance.currentDrink = null;

        CupController cup = PlayerInventory.Instance.currentCup;

        if (cup != null)
        {
            cup.ResetCupToStart();
        }
        else
        {
            Debug.LogWarning("currentCup не найден. Напиток удалён, но чашка не сброшена.");
        }
    }
}
