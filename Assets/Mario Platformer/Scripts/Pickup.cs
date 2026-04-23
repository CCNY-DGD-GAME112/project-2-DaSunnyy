using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Heart,
        Mushroom,
        FireFlower,
        Star,
    }

    public PickupType type;

    private void HandleStar(Collider other)
    {
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement == null) return;

        movement.TriggerVictory();

        AudioManager.Instance?.StopBGM();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Victory);

        GetComponent<Collider>().enabled = false;

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PowerUp power = other.GetComponent<PowerUp>();
        if (power == null) return;

        switch (type)
        {
            case PickupType.Heart:
                power.PickupHeart();
                ScoreManager.Instance?.AddScore(1000);
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.Heal);
                Destroy(gameObject);
                break;

            case PickupType.Mushroom:
                power.PickupMushroom();
                ScoreManager.Instance?.AddScore(1000);
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.PowerUp);
                Destroy(gameObject);
                break;

            case PickupType.FireFlower:
                power.PickupFireFlower();
                ScoreManager.Instance?.AddScore(1000);
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.PowerUp);
                Destroy(gameObject);
                break;

            case PickupType.Star:
                ScoreManager.Instance?.AddScore(5000);
                HandleStar(other);
                break;
        }
    }
}