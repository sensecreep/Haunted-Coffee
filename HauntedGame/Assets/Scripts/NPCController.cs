using UnityEngine;
using UnityEngine.AI;
using System;

public class NPCController : MonoBehaviour
{
    public string dialogueId;

    public Transform startPoint;
    public Transform cafeCounterPoint;
    public Transform exitPoint;

    public float talkDistance = 2f;

    private NavMeshAgent agent;
    private GameObject player;

    private bool reachedCafe = false;
    private bool playerIsNear = false;
    private bool questCompleted = false;
    private bool isLeaving = false;

    private Action onLeftCafe;

    public bool PlayerIsInTalkDistance => playerIsNear;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        GoToCafe();
    }

    private void Update()
    {
        if (isLeaving)
        {
            CheckArrivalToExit();
            return;
        }

        if (!reachedCafe)
        {
            CheckArrivalToCafe();
        }
        else if (!questCompleted)
        {
            CheckPlayerDistance();
        }
    }

    void GoToCafe()
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent не найден");
            return;
        }

        if (cafeCounterPoint == null)
        {
            Debug.LogError("cafeCounterPoint не назначен");
            return;
        }

        agent.isStopped = false;
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
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        playerIsNear = distance <= talkDistance;
    }

    public void LeaveCafe(Action callback)
    {
        if (isLeaving)
            return;

        questCompleted = true;
        isLeaving = true;
        playerIsNear = false;
        onLeftCafe = callback;

        if (exitPoint == null)
        {
            Debug.LogError("exitPoint не назначен. Удаляю клиента без ухода.");
            FinishLeaving();
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(exitPoint.position);

        Debug.Log("Клиент уходит к exitPoint");
    }

    void CheckArrivalToExit()
    {
        if (exitPoint == null)
        {
            FinishLeaving();
            return;
        }

        float distanceToExit = Vector3.Distance(transform.position, exitPoint.position);

        if (!agent.pathPending &&
            (agent.remainingDistance <= agent.stoppingDistance + 0.1f || distanceToExit <= 0.5f))
        {
            FinishLeaving();
        }
    }

    void FinishLeaving()
    {
        Debug.Log("Клиент ушёл, удаляем объект");

        onLeftCafe?.Invoke();

        Destroy(gameObject);
    }
}