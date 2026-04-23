using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;
    public int scoreValue = 100;

    private bool dead = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (dead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die(attacker);
        }
        else
        {
            Knockback(attacker);
        }
    }

    void Knockback(GameObject attacker)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 dir = (transform.position - attacker.transform.position).normalized;
        dir.y = 0.5f;

        float force = 5f;

        rb.AddForce(dir * force, ForceMode.Impulse);
    }

    void Die(GameObject attacker)
    {
        dead = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        ScoreManager.Instance?.AddScore(scoreValue);

        Destroy(gameObject);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.EnemyDie);
    }

    public bool IsDead() => dead;
}