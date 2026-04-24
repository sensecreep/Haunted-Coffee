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
        if (customer != null && !customer.CanStartDialogue())
            return;

        /*
        if (!dialogueBox.gameObject.activeSelf && dialogueStarted)
        {
            dialogueStarted = false;
            cameraController.EndDialogueLook();
        } */

        // Игрок не рядом — всё скрываем
        if (!npcController.PlayerIsInTalkDistance)
        {
            pressEUI.SetActive(false);
            dialogueStarted = false;
            return;
        }

        // Если диалог идёт — подсказку НЕ показываем
        if (dialogueBox.gameObject.activeSelf)
        {
            pressEUI.SetActive(false);
            return;
        }

        // Игрок рядом и диалог не идёт
        pressEUI.SetActive(true);

        if (dialogueStarted)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        playerController.enabled = false;

        cameraController.StartDialogueLook(npcLookPoint);

        dialogueStarted = true;
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
        if (customer != null)
        {
            customer.AcceptOrder();
            OrderUI.Instance.ShowOrder(customer.currentOrder);
        }
        playerController.enabled = true;
        dialogueStarted = false;
        cameraController.EndDialogueLook();
    }

}
