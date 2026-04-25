using UnityEngine;
using System.Collections;

public class FrogMovement : MonoBehaviour
{
    [Header("Ground Detection")]
    public float minAirTime = 0.2f;

    private float airTimer = 0f;

    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public float forwardForce = 4f;

    [Header("Turn Settings")]
    public float turnDuration = 0.2f;

    [Header("Random Jump Interval")]
    public float minJumpInterval = 1f;
    public float maxJumpInterval = 3f;

    [Header("Room Reference")]
    public Transform plane;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Visual Model")]
    public Transform frogModel;

    private Vector3 originalScale;

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

    private bool isWaitingToJump = false;
    private bool isJumping = false;

    private float roomMinX, roomMaxX, roomMinZ, roomMaxZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (frogModel == null)
            frogModel = transform;

        originalScale = frogModel.localScale;

        CalculateBounds();

        gameObject.layer = LayerMask.NameToLayer("Frog");
        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("Frog"),
            LayerMask.NameToLayer("Frog"),
            true
        );

        StartCoroutine(WaitThenJump());
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
        bool groundDetected = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        if (isJumping)
        {
            airTimer += Time.deltaTime;

            if (airTimer < minAirTime)
            {
                isGrounded = false;
            }
            else
            {
                isGrounded = groundDetected;
            }
        }
        else
        {
            isGrounded = groundDetected;
        }

        // Landing detection
        if (isGrounded && !wasGrounded && isJumping)
        {
            rb.linearVelocity = Vector3.zero;

            isJumping = false;
            airTimer = 0f;

            if (animator != null) animator.SetTrigger("Land");
            if (useScaleAnim) StartCoroutine(LandSquash());

            if (!isWaitingToJump)
            {
                StartCoroutine(WaitThenJump());
            }
        }

        wasGrounded = isGrounded;
    }

    IEnumerator WaitThenJump()
    {
        isWaitingToJump = true;

        float waitTime = Random.Range(minJumpInterval, maxJumpInterval);
        yield return new WaitForSeconds(waitTime);

        if (isGrounded && !isJumping)
        {
            Vector3 jumpDirection = ChooseDirection();
            yield return StartCoroutine(TurnToDirection(jumpDirection));
            Jump();
        }

        isWaitingToJump = false;
    }

    Vector3 ChooseDirection()
    {
        Vector3 pos = transform.position;
        float edgeBuffer = 2f;

        bool nearLeft = pos.x < roomMinX + edgeBuffer;
        bool nearRight = pos.x > roomMaxX - edgeBuffer;
        bool nearBottom = pos.z < roomMinZ + edgeBuffer;
        bool nearTop = pos.z > roomMaxZ - edgeBuffer;

        Vector3 targetDirection;

        if (nearLeft || nearRight || nearBottom || nearTop)
        {
            Vector3 inward = Vector3.zero;

            if (nearLeft) inward += Vector3.right;
            if (nearRight) inward += Vector3.left;
            if (nearBottom) inward += Vector3.forward;
            if (nearTop) inward += Vector3.back;

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

        Vector3 avoidance = GetAvoidanceDirection();
        targetDirection = (targetDirection + avoidance * avoidanceStrength).normalized;

        return targetDirection;
    }

    IEnumerator TurnToDirection(Vector3 jumpDirection)
    {
        if (jumpDirection == Vector3.zero)
            yield break;

        Quaternion startRot = transform.rotation;

        // LookRotation 默认让 Z 正方向朝向目标。
        // 你要让 Z 负方向朝向跳跃方向，所以这里用 -jumpDirection。
        Quaternion targetRot = Quaternion.LookRotation(-jumpDirection, Vector3.up);

        float time = 0f;

        while (time < turnDuration)
        {
            time += Time.deltaTime;
            float t = time / turnDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        transform.rotation = targetRot;
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
        isJumping = true;
        airTimer = 0f;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(
            -transform.forward * forwardForce + Vector3.up * jumpForce,
            ForceMode.Impulse
        );

        if (animator != null) animator.SetTrigger("Jump");
        if (useScaleAnim) StartCoroutine(JumpStretch());
    }

    IEnumerator JumpStretch()
    {
        frogModel.localScale = new Vector3(
            originalScale.x * 1.3f,
            originalScale.y * 0.6f,
            originalScale.z * 1.3f
        );

        yield return new WaitForSeconds(0.05f);

        frogModel.localScale = new Vector3(
            originalScale.x * 0.7f,
            originalScale.y * 1.4f,
            originalScale.z * 0.7f
        );

        yield return new WaitForSeconds(0.15f);

        frogModel.localScale = originalScale;
    }

    IEnumerator LandSquash()
    {
        frogModel.localScale = new Vector3(
            originalScale.x * 1.4f,
            originalScale.y * 0.6f,
            originalScale.z * 1.4f
        );

        yield return new WaitForSeconds(0.08f);

        frogModel.localScale = originalScale;
    }

    void LateUpdate()
    {
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