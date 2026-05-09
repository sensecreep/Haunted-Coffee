using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance;

    [Header("Spawn")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform cafeCounterPoint;
    public Transform exitPoint;

    [Header("UI")]
    public DialogeBox dialogueBox;
    public GameObject pressEUI;

    [Header("Camera")]
    public Transform npcLookPoint;

    [Header("Settings")]
    public float spawnDelay = 1.5f;

    private GameObject currentCustomer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnCustomer();
    }

    public void SpawnNextCustomer()
    {
        StartCoroutine(SpawnNextRoutine());
    }

    IEnumerator SpawnNextRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnCustomer();
    }

    void SpawnCustomer()
    {
        currentCustomer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);

        NPCController npcController = currentCustomer.GetComponent<NPCController>();
        NPCDialogueTrigger dialogueTrigger = currentCustomer.GetComponent<NPCDialogueTrigger>();

        npcController.cafeCounterPoint = cafeCounterPoint;
        npcController.exitPoint = exitPoint;
        //npcController.talkHint = pressEUI;

        dialogueTrigger.dialogueBox = dialogueBox;
        dialogueTrigger.pressEUI = pressEUI;

        Transform lookPoint = currentCustomer.transform.Find("LookPoint");

        if (lookPoint != null)
            dialogueTrigger.npcLookPoint = lookPoint;
        else
            dialogueTrigger.npcLookPoint = currentCustomer.transform;

        //dialogueTrigger.npcLookPoint = npcLookPoint;

        Debug.Log("Новый клиент заспавнен");
    }
}