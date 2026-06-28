using UnityEngine;

public class DrinkAssemblyTrigger : MonoBehaviour
{
    public GameObject pressEUI;

    [Header("Camera")]
    public Transform cameraPoint;
    public Transform playerSpot;

    [Header("Controller")]
    public DrinkAssemblyController controller;

    private bool playerInRange;
    private bool isUsing;
    public bool IsUsing => isUsing;

    void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);
    }

    void Update()
    {
        // 👉 ВХОД
        if (!isUsing && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenAssembly();
            return;
        }

        // 👉 ВЫХОД
        if (isUsing && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAssembly();
        }
    }

    void OpenAssembly()
    {
        isUsing = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        MovePlayerToSpot();

        PlayerLock.Instance.Lock();
        CameraFocusController.Instance.FocusOn(cameraPoint);

        controller.EnterMode();

        Debug.Log("Вошли в сборку напитка");
    }

    public void CloseAssembly()
    {
        isUsing = false;

        CameraFocusController.Instance.Return();
        PlayerLock.Instance.Unlock();

        controller.ExitMode();

        Debug.Log("Вышли из сборки напитка");
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

        if (pressEUI != null)
            pressEUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (pressEUI != null)
            pressEUI.SetActive(false);
    }
}