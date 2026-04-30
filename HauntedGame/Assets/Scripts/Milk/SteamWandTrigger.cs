using UnityEngine;

public class SteamWandTrigger : MonoBehaviour
{
    [Header("Visual")]
    public GameObject highlightObject; // дымка/outline

    [Header("Camera")]
    public Transform cameraPoint;
    public Transform playerSpot;

    [Header("Controller")]
    public SteamWandController steamController;

    private bool playerInRange = false;
    private bool isHovering = false;
    public bool isUsing = false;

    void Start()
    {
        if (highlightObject != null)
            highlightObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || isUsing)
            return;

        CheckHover();

        if (isHovering && Input.GetMouseButtonDown(0))
        {
            EnterSteamMode();
        }
    }

    void CheckHover()
    {
        Camera cam = CameraFocusController.Instance.GetActiveCamera();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            if (hit.transform.IsChildOf(transform))
            {
                if (!isHovering)
                {
                    isHovering = true;

                    if (highlightObject != null)
                        highlightObject.SetActive(true);
                }

                return;
            }
        }

        // если не попали
        if (isHovering)
        {
            isHovering = false;

            if (highlightObject != null)
                highlightObject.SetActive(false);
        }
    }

    void EnterSteamMode()
    {
        isUsing = true;

        if (highlightObject != null)
            highlightObject.SetActive(false);

        MovePlayer();

        PlayerLock.Instance.Lock();
        CameraFocusController.Instance.FocusOn(cameraPoint);

        if (steamController != null)
            steamController.enabled = true;
    }

    public void ExitSteamMode()
    {
        isUsing = false;

        if (steamController != null)
            steamController.enabled = false;

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
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (highlightObject != null)
            highlightObject.SetActive(false);
    }
}