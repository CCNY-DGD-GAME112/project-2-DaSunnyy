using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 8;
    public int currentHealth;

    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;
    public PlayerMovement movement;

    [Header("Damage")]
    public float knockbackForce = 4f;

    [Header("I-Frames")]
    public float iFrameDuration = 1.5f;
    private bool isInvincible = false;

    [Header("Visuals")]
    public Renderer[] renderers;
    private Material[] materials;
    private Color[] originalColors;

    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private bool isDead = false;
    private bool deathSequenceStarted = false;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        materials = new Material[renderers.Length];
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            originalColors[i] = materials[i].color;
        }

        currentHealth = maxHealth;

        if (movement == null)
            movement = GetComponent<PlayerMovement>();
    }

    private IEnumerator DoFlash()
    {
        for (int i = 0; i < materials.Length; i++)
            materials[i].color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < materials.Length; i++)
            materials[i].color = originalColors[i];
    }

    public void Flash()
    {
        StopCoroutine("DoFlash");
        StartCoroutine(DoFlash());
    }

    public void TakeDamage(int dmg, Vector3 knockbackDir)
    {
        if (isInvincible || isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        ApplyKnockback(knockbackDir);

        animator.SetTrigger("Hurt");
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Ouch);

        Flash();

        StartCoroutine(IFrameRoutine());

        if (currentHealth <= 0)
        {
            StartDeath();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.Die);

        }
    }

    private void ApplyKnockback(Vector3 dir)
    {
        if (rb == null) return;

        dir.y = Mathf.Abs(dir.y);
        rb.AddForce(dir.normalized * knockbackForce, ForceMode.Impulse);
    }


    private void StartDeath()
    {
        if (deathSequenceStarted) return;

        deathSequenceStarted = true;
        isDead = true;

        if (movement != null)
            movement.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        animator.SetTrigger("Hurt");
        animator.SetBool("isDead", true);

        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Defeat);


        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator IFrameRoutine()
    {
        isInvincible = true;

        StartCoroutine(IFrameFlash());

        yield return new WaitForSeconds(iFrameDuration);

        isInvincible = false;
    }

    private IEnumerator IFrameFlash()
    {
        float elapsed = 0f;
        float flashSpeed = 0.1f;

        while (elapsed < iFrameDuration)
        {
            SetAlpha(0.3f);
            yield return new WaitForSeconds(flashSpeed);

            SetAlpha(1f);
            yield return new WaitForSeconds(flashSpeed);

            elapsed += flashSpeed * 2f;
        }

        SetAlpha(1f);
    }

    private void SetAlpha(float alpha)
    {
        foreach (var mat in materials)
        {
            if (mat == null) continue;

            Color c = mat.color;
            c.a = alpha;
            mat.color = c;
        }
    }

    public void HealHeart()
    {
        currentHealth += 3;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Heal);
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
    }

    public void ApplyPowerUp()
    {
        FullHeal();
    }
}