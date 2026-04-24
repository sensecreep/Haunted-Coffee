using UnityEngine;

public class CoffeeMachineTrigger : MonoBehaviour
{
    public GameObject pressEUI;
    public Transform cameraPoint;
    public Transform playerSpot;
    public CoffeeMachineController machine;

    private bool playerInRange;
    private bool isUsing;

    void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);
    }

    void Update()
    {
        // ВХОД
        if (!isUsing && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenMachine();
            return;
        }

        // ВЫХОД
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMachine();
        }
    }

    void OpenMachine()
    {
        isUsing = true;

        pressEUI.SetActive(false);

        MovePlayerToSpot();

        PlayerLock.Instance.Lock();
        CameraFocusController.Instance.FocusOn(cameraPoint);

        machine.EnterMachineMode();

        Debug.Log("Вошли в кофемашину");
    }

    public void CloseMachine()
    {
        isUsing = false;

        CameraFocusController.Instance.Return();
        PlayerLock.Instance.Unlock();

        machine.ExitMachineMode();

        Debug.Log("Вышли из кофемашины");
    }

    void MovePlayerToSpot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = playerSpot.position;
        player.transform.rotation = playerSpot.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        pressEUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        pressEUI.SetActive(false);
    }
}