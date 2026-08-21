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
    public float slideSpeed = 15f;

    private float maxSlopeAngle = 45f;

    private float xRotation = 0f;

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
        bool tooSteep = IsGroundTooSteep();

        if (isGrounded && CheckClipping())
        {
            transform.position += Vector3.up * 0.1f;
        }

        if (isGrounded && !tooSteep)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (!isGrounded || !tooSteep)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;
            characterController.Move(move * speed * Time.deltaTime);
        }

        if (isGrounded && tooSteep)
        {
            Vector3 slideDirection = GetSlideDirection();
            characterController.Move(slideDirection * slideSpeed * Time.deltaTime);
            velocity.y = -slideSpeed;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }


    Vector3 GetSlideDirection()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal);
            return slideDir.normalized;
        }
        return Vector3.down;
    }

    bool CheckClipping()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            return true;
        }
        return false;
    }

    bool IsGroundTooSteep()
    {
        Vector3[] checkPoints = {
        transform.position,
        transform.position + transform.right * 0.2f,
        transform.position - transform.right * 0.2f,
        transform.position + transform.forward * 0.2f,
        transform.position - transform.forward * 0.2f
    };

        foreach (Vector3 point in checkPoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point, Vector3.down, out hit, 2f))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                Debug.Log("Slope Angle: " + slopeAngle);
                if (slopeAngle > maxSlopeAngle)
                    return true;
            }

            if (Physics.Raycast(point, transform.forward, out hit, 0.5f))
            {
                float wallAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (wallAngle > maxSlopeAngle)
                    return true;
            }
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
