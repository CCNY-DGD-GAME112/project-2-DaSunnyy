using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private PlayerMovement player;

    void Awake()
    {
        player = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(player.GetDamage(), gameObject);
        }
    }
}