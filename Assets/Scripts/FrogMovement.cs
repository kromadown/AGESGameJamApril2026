using UnityEngine;
using System.Collections;

public class FrogMovement : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public float forwardForce = 4f;
    public float jumpCooldown = 1.5f;

    [Header("Room Reference")]
    public Transform plane;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;
    public bool useScaleAnim = true;

    [Header("Frog Avoidance")]
    public float neighborRadius = 2.5f;
    public float avoidanceStrength = 1.5f;
    public LayerMask frogMask;

    private Rigidbody rb;
    private bool isGrounded;
    private bool wasGrounded;
    private float jumpTimer;

    private float roomMinX, roomMaxX, roomMinZ, roomMaxZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jumpTimer = jumpCooldown;

        CalculateBounds();

        // Frog layer (no self-collision)
        gameObject.layer = LayerMask.NameToLayer("Frog");
        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("Frog"),
            LayerMask.NameToLayer("Frog"),
            true
        );
    }

    void CalculateBounds()
    {
        float width = plane.localScale.x * 10f;
        float length = plane.localScale.z * 10f;

        Vector3 center = plane.position;

        roomMinX = center.x - width / 2f;
        roomMaxX = center.x + width / 2f;
        roomMinZ = center.z - length / 2f;
        roomMaxZ = center.z + length / 2f;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Landing detection
        if (isGrounded && !wasGrounded)
        {
            if (animator != null) animator.SetTrigger("Land");
            if (useScaleAnim) StartCoroutine(LandSquash());
        }

        wasGrounded = isGrounded;

        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0f && isGrounded)
        {
            ChooseDirection();
            Jump();
            jumpTimer = jumpCooldown;
        }
    }

    void ChooseDirection()
    {
        Vector3 pos = transform.position;
        float edgeBuffer = 2f;

        bool nearLeft   = pos.x < roomMinX + edgeBuffer;
        bool nearRight  = pos.x > roomMaxX - edgeBuffer;
        bool nearBottom = pos.z < roomMinZ + edgeBuffer;
        bool nearTop    = pos.z > roomMaxZ - edgeBuffer;

        Vector3 targetDirection;

        if (nearLeft || nearRight || nearBottom || nearTop)
        {
            Vector3 inward = Vector3.zero;

            if (nearLeft)   inward += Vector3.right;
            if (nearRight)  inward += Vector3.left;
            if (nearBottom) inward += Vector3.forward;
            if (nearTop)    inward += Vector3.back;

            inward.Normalize();

            Vector3 random = new Vector3(
                Random.Range(-0.3f, 0.3f),
                0,
                Random.Range(-0.3f, 0.3f)
            );

            targetDirection = (inward * 0.8f + random * 0.2f).normalized;
        }
        else
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            targetDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }

        // Avoid other frogs (future prediction)
        Vector3 avoidance = GetAvoidanceDirection();
        targetDirection = (targetDirection + avoidance * avoidanceStrength).normalized;

        if (targetDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(targetDirection);
    }

    Vector3 GetAvoidanceDirection()
    {
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius, frogMask);

        Vector3 avoidance = Vector3.zero;

        foreach (Collider col in neighbors)
        {
            if (col.gameObject == gameObject) continue;

            Rigidbody otherRb = col.attachedRigidbody;
            if (otherRb == null) continue;

            Vector3 futurePos = col.transform.position + otherRb.linearVelocity * 0.5f;

            Vector3 away = transform.position - futurePos;
            float dist = away.magnitude;

            if (dist > 0.01f)
                avoidance += away.normalized / dist;
        }

        return avoidance.normalized;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.forward * forwardForce + Vector3.up * jumpForce, ForceMode.Impulse);

        if (animator != null) animator.SetTrigger("Jump");
        if (useScaleAnim) StartCoroutine(JumpStretch());
    }

    IEnumerator JumpStretch()
    {
        transform.localScale = new Vector3(1.3f, 0.6f, 1.3f);
        yield return new WaitForSeconds(0.05f);

        transform.localScale = new Vector3(0.7f, 1.4f, 0.7f);
        yield return new WaitForSeconds(0.15f);

        transform.localScale = Vector3.one;
    }

    IEnumerator LandSquash()
    {
        transform.localScale = new Vector3(1.4f, 0.6f, 1.4f);
        yield return new WaitForSeconds(0.08f);

        transform.localScale = Vector3.one;
    }

    void LateUpdate()
    {
        // Soft clamp (safety net, not primary containment)
        Vector3 pos = transform.position;
        float padding = 0.2f;

        pos.x = Mathf.Clamp(pos.x, roomMinX + padding, roomMaxX - padding);
        pos.z = Mathf.Clamp(pos.z, roomMinZ + padding, roomMaxZ - padding);

        transform.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        if (plane != null)
        {
            float width = plane.localScale.x * 10f;
            float length = plane.localScale.z * 10f;

            Vector3 center = plane.position;
            Vector3 size = new Vector3(width, 1f, length);

            Gizmos.DrawWireCube(center, size);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, neighborRadius);
    }
}