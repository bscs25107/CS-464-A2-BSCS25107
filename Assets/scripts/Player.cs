using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 1.8f;

    [Header("Ground Check Settings")]
    [SerializeField] private float checkRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;

    [Header("Jump Assist Settings")]
    [SerializeField] private float coyoteTime = 0.15f;    // Time window after leaving ground
    [SerializeField] private float jumpBufferTime = 0.15f; // Time window before landing

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector3 moveInput;
    private bool isGrounded;
    private Transform camTransform;

    // Timers
    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    // Calculates the bottom point of the CapsuleCollider in World Space
    private Vector3 GetBottomPosition()
    {
        Vector3 center = transform.TransformPoint(capsule.center);
        float bottomOffset = (capsule.height * 0.5f) - capsule.radius;
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
    }
}