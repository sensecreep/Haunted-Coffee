using UnityEngine;

public class MilkStationTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject pressEUI;

    [Header("Links")]
    public MilkController milkController;

    [Header("Camera")]
    public Transform cameraPoint;

    [Header("Player")]
    public Transform playerSpot;

    private bool playerInRange;
    private bool isUsing;

    void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || isUsing)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            EnterMilkStation();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitMilkStation();
        }
    }

    void EnterMilkStation()
    {
        isUsing = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        MovePlayerToSpot();

        PlayerLock.Instance.Lock();

        CameraFocusController.Instance.FocusOn(cameraPoint);

        milkController.EnterMilkMode();
    }

    public void ExitMilkStation()
    {
        isUsing = false;

        milkController.ExitMilkMode();

        CameraFocusController.Instance.Return();
        PlayerLock.Instance.Unlock();

        if (pressEUI != null)
            pressEUI.SetActive(true);
    }

    void MovePlayerToSpot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = playerSpot.position;
        player.transform.rotation = playerSpot.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (pressEUI != null)
            pressEUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (pressEUI != null)
            pressEUI.SetActive(false);
    }
}