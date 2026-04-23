using UnityEngine;

public class Goomba : MonoBehaviour, PlayerMovement.IEnemy
{
    public float moveSpeed = 2f;
    public float visionRange = 50f;
    public float stopDistance = 1.2f;

    public LayerMask whatIsPlayer;
    public float detectHeightOffset = 0.5f;

    private Transform player;
    private Rigidbody rb;
    private EnemyHealth health;

    private bool playerInSight;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<EnemyHealth>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void FixedUpdate()
    {
        if (player == null || health.IsDead()) return;

        playerInSight = Physics.CheckSphere(
            transform.position + Vector3.up * detectHeightOffset,
            visionRange,
            whatIsPlayer
        );

        if (!playerInSight)
        {
            Stop();
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        if (distance > stopDistance)
        {
            ChasePlayer(toPlayer.normalized);
        }
        else
        {
            Stop();
        }
    }

    void ChasePlayer(Vector3 dir)
    {
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        transform.forward = dir;
    }

    void Stop()
    {
        Vector3 vel = rb.linearVelocity;
        vel.x = 0f;
        vel.z = 0f;
        rb.linearVelocity = vel;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (health != null)
        {
            health.TakeDamage(damage, attacker);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * detectHeightOffset, visionRange);
    }
}