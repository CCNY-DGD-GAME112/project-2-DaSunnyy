using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StarGoal : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;

    [Header("Scene Flow")]
    [SerializeField] private float poseDuration = 3f;
    [SerializeField] private bool restartScene = true;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;

            GetComponent<Collider>().enabled = false;

            AudioManager.Instance?.StopBGM();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.Victory);

            StartCoroutine(VictorySequence(other));
        }
    }

    private IEnumerator VictorySequence(Collider player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();

        while (controller != null && !controller.isGrounded)
            yield return null;

        playerAnimator.SetBool("Victory", true);

        yield return new WaitForSeconds(0.5f);

        playerAnimator.SetBool("Pose", true);

        yield return new WaitForSeconds(poseDuration);

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);

        EndLevel();
    }

    private void EndLevel()
    {
        if (restartScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}