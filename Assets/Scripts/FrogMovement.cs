using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    public bool isMatingLocked = false;

    [Header("Wandering")]
    public float jumpForce = 6f;
    public float forwardForce = 4f;
    public float minJumpInterval = 1f;
    public float maxJumpInterval = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded;
    private float wanderTimer;

    [Header("Mate Movement")]
    public bool moveToMatingPoint = false;
    public Vector3 matingPoint;
    public float stopDistance = 1.5f;
    public float hopCooldown = 0.7f;

    private float hopTimer;

    public bool IsGrounded => isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 🧠 HARD SAFETY CHECK (prevents kinematic crash)
        if (rb == null || isMatingLocked)
            return;

        CheckGround();

        if (moveToMatingPoint)
        {
            MateHop();
        }
        else
        {
            Wander();
        }
    }

    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (!isGrounded) return;

        if (wanderTimer <= 0f)
        {
            Vector3 dir = Random.insideUnitSphere;
            dir.y = 0;
            dir.Normalize();

            transform.rotation = Quaternion.LookRotation(dir);

            SafeClearVelocity();

            rb.AddForce(
                dir * forwardForce + Vector3.up * jumpForce,
                ForceMode.Impulse
            );

            wanderTimer = Random.Range(minJumpInterval, maxJumpInterval);
        }
    }

    void MateHop()
    {
        if (!isGrounded) return;

        hopTimer -= Time.deltaTime;

        Vector3 dir = matingPoint - transform.position;
        dir.y = 0;

        float dist = dir.magnitude;

        if (dist <= stopDistance)
        {
            SafeClearVelocity();
            return;
        }

        if (hopTimer > 0f) return;

        dir.Normalize();

        transform.rotation = Quaternion.LookRotation(dir);

        SafeClearVelocity();

        rb.AddForce(
            dir * forwardForce + Vector3.up * jumpForce,
            ForceMode.Impulse
        );

        hopTimer = hopCooldown;
    }

    // 🧠 CENTRAL SAFE FUNCTION (prevents kinematic crash)
    void SafeClearVelocity()
    {
        if (rb == null) return;

        // only allow velocity writes if NOT kinematic
        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}