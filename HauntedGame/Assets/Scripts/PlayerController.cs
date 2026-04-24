using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 7f;
    public float gravity = -40f;
    public float jumpHeight = 1.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.15f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Проверка земли
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // прижимаем к земле
        }

        // Ввод WASD
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Направление движения
        Vector3 move = transform.right * x + transform.forward * z;

        // Бег
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Движение
        controller.Move(move * speed * Time.deltaTime);

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Гравитация
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
