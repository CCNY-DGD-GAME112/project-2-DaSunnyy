using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth player;

    public Image healthImage;

    public Sprite[] healthSprites;

    private int lastHealth = -1;

    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerHealth>();

        UpdateUI(true);
    }

    void Update()
    {
        if (player == null) return;

        if (player.currentHealth != lastHealth)
        {
            UpdateUI();
        }
    }

    void UpdateUI(bool force = false)
    {
        if (healthSprites == null || healthSprites.Length == 0 || healthImage == null)
            return;

        int clamped = Mathf.Clamp(player.currentHealth, 0, player.maxHealth);

        int spriteIndex = (player.maxHealth - clamped);

        spriteIndex = Mathf.Clamp(spriteIndex, 0, healthSprites.Length - 1);

        healthImage.sprite = healthSprites[spriteIndex];

        lastHealth = player.currentHealth;
    }
}