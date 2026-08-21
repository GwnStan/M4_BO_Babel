using UnityEngine;

public class player_movement : MonoBehaviour
{
    public CharacterController characterController;
    public Camera playerCamera;

    public float speed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public bool canMove = true;
    private Vector3 velocity;

    private float maxSlopeAngle = 45f;

    private float xRotation = 0f;
    public float mouseSensitivity = 2f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        handleMovement();

        handleMouseLook();
    }

    void handleMovement()
    {
        bool isGrounded = characterController.isGrounded;

        if (isGrounded && !IsGroundTooSteep())
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            characterController.Move(move * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    bool IsGroundTooSteep()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            return slopeAngle > maxSlopeAngle;
        }
        return false;
    }


    void handleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(0f, mouseX, 0f, Space.Self);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
