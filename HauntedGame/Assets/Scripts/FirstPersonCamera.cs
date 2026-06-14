using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float sensitivity = 300f;
    public Transform playerBody;
    public float xRotation = 0f;

    void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", sensitivity);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // ѕоворот камеры по вертикали
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f); // защита от переворота
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ѕоворот игрока по горизонтали
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public float CurrentXRotation => xRotation;

    public void SetXRotation(float value)
    {
        xRotation = value;
    }
}
