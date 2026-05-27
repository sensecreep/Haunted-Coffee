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
        if (!isUsing && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            EnterMilkStation();
            return;
        }

        if (isUsing && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitMilkStation();
        }
    }

    void EnterMilkStation()
    {
        if (isUsing)
            return;

        isUsing = true;

        if (pressEUI != null)
            pressEUI.SetActive(false);

        MovePlayerToSpot();

        PlayerLock.Instance.Lock();
        CameraFocusController.Instance.FocusOn(cameraPoint);

        if (milkController != null)
        {
            if (milkController.pitcher != null)
                milkController.pitcher.currentStation = this;

            milkController.EnterMilkMode();
        }

        Debug.Log("Вошли на станцию молока");
    }

    public void ExitMilkStation()
    {
        if (!isUsing)
            return;

        isUsing = false;

        if (milkController != null)
            milkController.ExitMilkMode();

        CameraFocusController.Instance.Return();
        PlayerLock.Instance.Unlock();

        if (pressEUI != null && playerInRange)
            pressEUI.SetActive(true);

        Debug.Log("Вышли со станции молока");
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

        if (pressEUI != null && !isUsing)
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