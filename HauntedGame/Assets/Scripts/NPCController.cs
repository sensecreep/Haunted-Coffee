using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public string dialogueId;

    public Transform startPoint;
    public Transform cafeCounterPoint;

    public float talkDistance = 2f;
    public GameObject talkHint;

    private NavMeshAgent agent;
    private GameObject player;

    private bool reachedCafe = false;
    private bool playerIsNear = false;
    private bool questCompleted = false;

    public bool PlayerIsInTalkDistance => playerIsNear;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        talkHint.SetActive(false);

        GoToCafe();
    }

    private void Update()
    {
        if (!reachedCafe)
        {
            CheckArrivalToCafe();
        }
        else if (!questCompleted)
        {
            CheckPlayerDistance();
            //CheckPlayerInteraction();
        }
    }

    void GoToCafe()
    {
        agent.SetDestination(cafeCounterPoint.position);
    }

    void CheckArrivalToCafe()
    {
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            reachedCafe = true;
            agent.isStopped = true;

        }
    }

    void CheckPlayerDistance()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= talkDistance)
        {
            if (!playerIsNear)
            {
                playerIsNear = true;
                talkHint.SetActive(true);
            }
        }
        else
        {
            if (playerIsNear)
            {
                playerIsNear = false;
                talkHint.SetActive(false);
            }
        }
    }
    /*
    void CheckPlayerInteraction()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }

    
    void StartDialogue()
    {
        questCompleted = true;
        talkHint.SetActive(false);

        var dialogBox = FindObjectOfType<DialogeBox>(true);
        var lines = DialogueManager.Instance.GetDialogue(dialogueId);

        dialogBox.gameObject.SetActive(true);
        dialogBox.StartDialogue(lines);
    }
    */
}
