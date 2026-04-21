using UnityEngine;

public class JumpPanel : MonoBehaviour
{
    [Header("Jump Settings")]
    public float bounceForce = 40f;

    [Header("Detection")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        Animator animator = other.GetComponent<Animator>();
        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        if (rb == null) return;

        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;
        rb.linearVelocity = vel;


        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);

        if (animator != null)
        {
            animator.CrossFade("Mario_Jump", 0.05f);

            AudioManager.Instance?.PlaySFX(AudioManager.Instance.Boing);
        }

        if (movement != null)
        {
            movement.jumpHeld = true;
        }
    }
}