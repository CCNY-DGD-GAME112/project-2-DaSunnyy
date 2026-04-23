using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 1;
    public Transform hitboxOrigin;

    void Awake()
    {
        if (hitboxOrigin == null)
            hitboxOrigin = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null) return;

        Vector3 knockDir = (other.transform.position - hitboxOrigin.position).normalized;

        ph.TakeDamage(damage, knockDir);
    }
}