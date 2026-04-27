using UnityEngine;
using TMPro;

public class PowerUpUI : MonoBehaviour
{
    public PowerUp powerUp;

    public TMP_Text mushroomText;
    public TMP_Text fireText;

    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        mushroomText.text = powerUp.IsMushroomActive()
            ? "Mushroom: " + powerUp.GetMushroomTime().ToString("F1")
            : "";

        fireText.text = powerUp.IsFireActive()
            ? "Fire: " + powerUp.GetFireTime().ToString("F1")
            : "";
    }
}