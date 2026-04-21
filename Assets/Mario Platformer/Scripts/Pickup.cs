using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Heart,
        Mushroom,
        FireFlower
    }

    public PickupType type;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PowerUp power = other.GetComponent<PowerUp>();
        if (power == null) return;

        switch (type)
        {
            case PickupType.Heart:
                power.PickupHeart();
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.Heal);
                break;

            case PickupType.Mushroom:
                power.PickupMushroom();
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.PowerUp);
                break;

            case PickupType.FireFlower:
                power.PickupFireFlower();
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.PowerUp);
                break;
        }
        Destroy(gameObject);
    }
}