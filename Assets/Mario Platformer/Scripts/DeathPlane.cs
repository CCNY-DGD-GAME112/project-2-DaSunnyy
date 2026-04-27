using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathPlane : MonoBehaviour
{
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            return;
        }

        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
            health.currentHealth = 0;

        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        other.gameObject.SetActive(false);

        AudioManager.Instance?.StopBGM();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Die);

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Defeat);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}