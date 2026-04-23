using UnityEngine;

public class StompHurtbox : MonoBehaviour
{
    private EnemyHealth enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player == null) return;

        if (player.IsFalling())
        {
            player.TriggerStomp(enemy.gameObject);
        }
    }
}