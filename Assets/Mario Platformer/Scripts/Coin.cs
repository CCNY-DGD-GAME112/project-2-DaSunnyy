using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum CoinType
    {
        Single,
        Stack5,
        Stack10
    }

    public CoinType type;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        int value = 0;

        switch (type)
        {
            case CoinType.Single:
                value = 200;
                break;
            case CoinType.Stack5:
                value = 1000;
                break;
            case CoinType.Stack10:
                value = 2000;
                break;
        }

        ScoreManager.Instance?.AddScore(value);

        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Coin);

        Destroy(gameObject);
    }
}