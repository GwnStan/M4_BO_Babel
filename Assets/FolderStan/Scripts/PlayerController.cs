using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 12f;
    [SerializeField] float runSpeed = 20f;
    [SerializeField] float crouchSpeed = 8f;
    [SerializeField] float airControl = 0.1f;
    [SerializeField] float acceleration = 25f;
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float gravity = 16f;
    [SerializeField] float fallMultiplier = 1.5f;

    [Header("Ground")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float maxSlopeAngle = 45f;

    [Header("Crouch")]
    [SerializeField] float standHeight = 2f;
    [SerializeField] float crouchHeight = 1f;

    Rigidbody rb;
    CapsuleCollider capsule;

    bool grounded;
    bool jumpQueued;
    Vector3 groundNormal = Vector3.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpQueued = true;

        float targetHeight = Input.GetButton("Crouch") ? crouchHeight : standHeight;

        capsule.height = targetHeight;
    }

    void FixedUpdate()
    {
        GroundCheck();

        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        Vector3 wishDir =
            (orientation.forward * input.y + orientation.right * input.x).normalized;

        float speed =
            Input.GetButton("Crouch") ? crouchSpeed :
            Input.GetButton("Run") ? runSpeed :
            walkSpeed;

        Vector3 targetVel = wishDir * speed;

        Vector3 vel = rb.linearVelocity;
        Vector3 flatVel = new Vector3(vel.x, 0f, vel.z);

        float accel = grounded ? acceleration : acceleration * airControl;

        Vector3 change = targetVel - flatVel;
        rb.AddForce(change * accel, ForceMode.Acceleration);

        // --- JUMP ---
        if (jumpQueued && grounded)
        {
            rb.linearVelocity = new Vector3(vel.x, 0f, vel.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        jumpQueued = false;

        if (!grounded)
        {
            float extraGravity = gravity * (rb.linearVelocity.y < 0 ? fallMultiplier : 1f);
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
    }

    void GroundCheck()
    {
        grounded = false;
        groundNormal = Vector3.up;

        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.SphereCast(
            origin,
            capsule.radius * 0.9f,
            Vector3.down,
            out RaycastHit hit,
            capsule.bounds.extents.y + groundCheckDistance,
            groundLayer
        ))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlopeAngle)
            {
                grounded = true;
                groundNormal = hit.normal;
            }
        }
    }
}