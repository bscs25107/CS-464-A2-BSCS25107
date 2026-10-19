using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 1.8f;

    [Header("Ground Check Settings")]
    [SerializeField] private float checkRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;

    [Header("Jump Assist Settings (Part B)")]
    [SerializeField] private float coyoteTime = 0.1f;    // Time window after leaving ground
    [SerializeField] private float jumpBufferTime = 0.12f; // Time window before landing
    [SerializeField] private float fallMultiplier = 2.2f;  // B2: extra gravity while falling

    [Header("Juice: Landing Squash (B3)")]
    [SerializeField] private Transform body;               // child mesh, NO collider
    [SerializeField] private float squashY = 0.7f;
    [SerializeField] private float squashDuration = 0.25f;
    [SerializeField] private float minAirTime = 0.1f;      // only real landings

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector3 moveInput;
    private bool isGrounded, wasGrounded, jumpQueued;
    private Transform camTransform;
    private Coroutine squashRoutine;
    private float airTime;

    // Timers
    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;                              // no tipping over
        rb.interpolation = RigidbodyInterpolation.Interpolate; // smooth visuals

        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    // World-space centre of a sphere whose lowest point is 0.05 below the capsule's feet
    private Vector3 GetBottomPosition()
    {
        Vector3 center = transform.TransformPoint(capsule.center);
        float bottomOffset = (capsule.height * 0.5f) - checkRadius + 0.05f;
        return center - (transform.up * bottomOffset);
    }

    private void Update()
    {
        // 1. Movement Inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 2. Camera-Relative Direction
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        if (camTransform != null)
        {
            forward = camTransform.forward;
            right = camTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        moveInput = Vector3.ClampMagnitude(forward * vertical + right * horizontal, 1f);

        // 3. Ground Detection
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(GetBottomPosition(), checkRadius, groundMask);

        // Juice: only on a real landing (more than minAirTime in the air)
        if (!isGrounded)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            if (!wasGrounded && airTime > minAirTime && body != null)
            {
                if (squashRoutine != null) StopCoroutine(squashRoutine);
                squashRoutine = StartCoroutine(Squash());
            }
            airTime = 0f;
        }

        // 4. Coyote timer: refill only if grounded, not rising, no jump queued
        if (isGrounded && !jumpQueued && rb.linearVelocity.y <= 0.01f)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // 5. Jump buffer timer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // 6. Queue the jump when BOTH windows are open, and use both up
        if (!jumpQueued && jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            jumpQueued = true;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, moveInput.z * speed);

        // Apply the queued jump once
        if (jumpQueued)
        {
            ExecuteJump();
            jumpQueued = false;
        }

        // B2: fall faster than you rise (same apex, shorter fall)
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void ExecuteJump()
    {
        float g = -Physics.gravity.y;
        float v0 = Mathf.Sqrt(2f * g * jumpHeight);

        // Reset vertical velocity first, so a jump taken while falling isn't short
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * (rb.mass * v0), ForceMode.Impulse);
    }

    // B3: squash on landing, eased with OutBack, ends exactly at rest
    private IEnumerator Squash()
    {
        for (float elapsed = 0f; elapsed < squashDuration; elapsed += Time.deltaTime)
        {
            float p = OutBack(elapsed / squashDuration);
            float y = Mathf.LerpUnclamped(squashY, 1f, p);
            float xz = 1f / Mathf.Sqrt(y); // keep the volume
            body.localScale = new Vector3(xz, y, xz);
            yield return null;
        }
        body.localScale = Vector3.one; // exactly at rest
        squashRoutine = null;
    }

    private static float OutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        float u = t - 1f;
        return 1f + c3 * u * u * u + c1 * u * u;
    }

    private void OnDrawGizmosSelected()
    {
        if (capsule == null) capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(GetBottomPosition(), checkRadius);
        }
    }
}
