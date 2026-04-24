using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 7f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;
    private bool jumpLocked;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("IsJumping", false);
            jumpLocked = false;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded && !jumpLocked)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping", true);
            jumpLocked = true;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove =
            move * speed + Vector3.up * velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        float animationSpeed = move.magnitude * (isSprinting ? 1f : 0.5f);
        animator.SetFloat("Speed", animationSpeed);
    }
}
