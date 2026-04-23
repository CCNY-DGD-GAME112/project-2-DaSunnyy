using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PlayerHealth health;
    public PlayerMovement movement;

    public GameObject fireballPrefab;
    public Transform firePoint;
    public MouseCamera cam;

    public float mushroomDuration = 30f;
    public float mushroomScaleMultiplier = 2f;

    public float fireDuration = 30f;
    public float fireCooldown = 0.3f;

    public bool IsMushroomActive() => mushroomActive;
    public bool IsFireActive() => fireActive;

    public float GetMushroomTime() => mushroomTimer;
    public float GetFireTime() => fireTimer;

    private bool mushroomActive;
    private bool fireActive;

    private float mushroomTimer;
    private float fireTimer;

    private float lastFireTime;

    private Vector3 originalScale;

    void Awake()
    {
        if (health == null) health = GetComponent<PlayerHealth>();
        if (movement == null) movement = GetComponent<PlayerMovement>();

        originalScale = transform.localScale;
    }

    void Update()
    {
        HandleTimers();
    }

    public void PickupHeart()
    {
        health.HealHeart();
    }

    public void PickupMushroom()
    {
        health.FullHeal();

        mushroomTimer = mushroomDuration;

        if (!mushroomActive)
        {
            mushroomActive = true;
            ActivateMushroom();
        }
    }

    public void PickupFireFlower()
    {
        health.FullHeal();

        fireTimer = fireDuration;
        fireActive = true;
    }

    void ActivateMushroom()
    {
        transform.localScale = originalScale * mushroomScaleMultiplier;

        if (movement != null)
            movement.SetPoweredUp(true);
    }

    void DeactivateMushroom()
    {
        transform.localScale = originalScale;

        if (movement != null)
            movement.SetPoweredUp(false);

        mushroomActive = false;
    }

    public void TryShootFireball()
    {
        if (!fireActive) return;

        if (Time.time - lastFireTime < fireCooldown) return;

        lastFireTime = Time.time;

        if (fireballPrefab != null && firePoint != null && cam != null)
        {
            Vector3 direction = cam.transform.forward;
            direction.y = 0f; 

            if (direction.sqrMagnitude < 0.01f)
                direction = transform.forward;

            Quaternion rot = Quaternion.LookRotation(direction);

            Instantiate(fireballPrefab, firePoint.position, rot);
        }
    }

    public void FireballEvent()
    {
        TryShootFireball();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Fireball);
    }

    void DeactivateFire()
    {
        fireActive = false;
    }

    void HandleTimers()
    {
        if (mushroomActive)
        {
            mushroomTimer -= Time.deltaTime;

            if (mushroomTimer <= 0f)
            {
                DeactivateMushroom();
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.PowerDown);
            }
        }

        if (fireActive)
        {
            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0f)
            {
                DeactivateFire();
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.PowerDown);
            }
        }
    }
}