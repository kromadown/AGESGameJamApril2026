using UnityEngine;

public class FrogBounceZone : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceForce = 10f;

    [Header("Sound Settings")]
    public AudioSource bounceSound;

    [Header("Layer")]
    public LayerMask frogLayer;

    [Header("Animator")]
    public Animator bounceAnim;

    private void OnTriggerEnter(Collider other)
    {
        // 检查是不是 Frog 层
        if ((frogLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        bounceSound.pitch = Random.Range(0.95f, 1.05f);
        bounceSound.PlayOneShot(bounceSound.clip);

        bounceAnim.SetTrigger("Bounce");

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // 本地 Y 轴方向（重点✨）
        Vector3 bounceDir = transform.up;

        rb.linearVelocity = Vector3.zero; // 防止原速度干扰（可选）
        rb.AddForce(bounceDir * bounceForce, ForceMode.Impulse);
    }
}