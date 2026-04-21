using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 3f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        PlayerMovement.IEnemy enemy = other.GetComponentInParent<PlayerMovement.IEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage, gameObject);

            AudioManager.Instance?.PlaySFX(AudioManager.Instance.FireballHit);

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}