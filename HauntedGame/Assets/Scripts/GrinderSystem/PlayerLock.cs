using UnityEngine;

public class PlayerLock : MonoBehaviour
{
    public static PlayerLock Instance;

    public FirstPersonCamera cameraController;
    public PlayerController playerMovement;

    void Awake()
    {
        Instance = this;
    }

    public void Lock()
    {
        cameraController.enabled = false;
        playerMovement.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Unlock()
    {
        cameraController.enabled = true;
        playerMovement.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
