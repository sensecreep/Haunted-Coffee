using UnityEngine;

public class DrinkAssemblyTrigger : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraPoint;
    public Transform playerSpot;

    [Header("Controller")]
    public DrinkAssemblyController controller;

    private bool playerInRange;
    public bool isUsing;

    void Update()
    {
        if (!playerInRange || isUsing)
            return;

        // вход по ЛКМ (как ты хочешь)
        if (Input.GetMouseButtonDown(0))
        {
            TryEnter();
        }
    }

    void TryEnter()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
            return;

        if (!hit.transform.IsChildOf(transform))
            return;

        Enter();
    }

    void Enter()
    {
        isUsing = true;

        MovePlayer();

        PlayerLock.Instance.Lock();
        CameraFocusController.Instance.FocusOn(cameraPoint);

        controller.EnterMode();
    }

    public void Exit()
    {
        isUsing = false;

        controller.ExitMode();

        CameraFocusController.Instance.Return();
        PlayerLock.Instance.Unlock();
    }

    void MovePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = playerSpot.position;
        player.transform.rotation = playerSpot.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }
}