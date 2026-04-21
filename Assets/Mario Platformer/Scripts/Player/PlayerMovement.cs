using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 50f;
    public float jumpForce = 20f;
    public float runMultiplier = 2000f;
    public float rotationSpeed = 360f;
    public float fallMultiplier = 5f;
    public float lowJumpMultiplier = 2.5f;
    public float stompBounceForce = 50f;
    public Transform cameraTransform;
    public PowerUp powerUp;

    public GameObject AttackHitbox;
    public int Damage = 1;
    public float MushroomMultiplier = 2f;
    public float comboResetTime = 0.8f;

    private bool attackQueued;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Animator animator;
    private float stompCooldown = 0.1f;
    private float lastStompTime;

    public bool isRunning;
    public bool isGrounded;
    public bool jumpHeld;

    private int comboStep = 0;
    private float lastAttackTime;
    private bool canChain;

    private bool isPoweredUp = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        powerUp = GetComponent<PowerUp>();

        if (AttackHitbox != null)
            AttackHitbox.SetActive(false);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        {
            if (AttackHitbox.activeSelf)
                AttackHitbox.SetActive(false);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public float VerticalVelocity()
    {
        return rb.linearVelocity.y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            animator.SetTrigger("Jump");

            AudioManager.Instance?.PlaySFX(AudioManager.Instance.Jump);

            rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y),
            rb.linearVelocity.z
            );
            jumpHeld = true;
        }

        if (context.canceled)
        {
            jumpHeld = false;
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        AttackInput();
    }

    public void Fireball(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (powerUp == null) return;

        if (!powerUp.IsFireActive()) return;

        animator.SetTrigger("Fire");
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Fireball);
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        Debug.Log(Time.timeScale);

        if (comboStep == 3)
        {
            if (state.IsName("Mario_Attack3") &&
                state.normalizedTime >= 1f &&
                !animator.IsInTransition(0))
            {
                ResetCombo();
            }
        }

        if (comboStep > 0 && Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    public interface IEnemy
    {
        void TakeDamage(int damage, GameObject attacker);
    }

    public void HandleStompHit(GameObject enemyObject)
    {
        if (enemyObject == null) return;

        IEnemy enemy = enemyObject.GetComponent<IEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(GetDamage(), gameObject);
        }
    }

    void FixedUpdate()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * moveInput.y + camRight * moveInput.x;
        bool hasInput = move.sqrMagnitude > 0.01f;

        if (hasInput)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        Vector3 vel = rb.linearVelocity;

        Vector3 moveDir = new Vector3(move.x, 0f, move.z).normalized;

        vel.x = moveDir.x * speed;
        vel.z = moveDir.z * speed;

        rb.linearVelocity = vel;

        bool isMoving = move.sqrMagnitude > 0.01f;

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isFalling", !isGrounded && vel.y < -0.2f);

        animator.SetBool("isWalking", isMoving && !isRunning && isGrounded);
        animator.SetBool("isRunning", isMoving && isRunning && isGrounded);
    }

    IEnumerator SetGroundedNextFrame(bool value)
    {
        yield return null;
        isGrounded = value;
    }

    public void TriggerStomp(GameObject enemy)
    {
        if (Time.time - lastStompTime < stompCooldown) return;

        lastStompTime = Time.time;

        animator.CrossFade("Mario_Stomp", 0.05f);

        HandleStompHit(enemy);

        AudioManager.Instance?.PlaySFX(AudioManager.Instance.Stomp);

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            stompBounceForce,
            rb.linearVelocity.z
        );
    }

    public void AttackInput()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        bool notInAttack =
            !state.IsName("Mario_Attack1") &&
            !state.IsName("Mario_Attack2") &&
            !state.IsName("Mario_Attack3");

        if (comboStep == 0 || notInAttack)
        {
            StartCombo();
            return;
        }

        if (canChain)
        {
            ContinueCombo();
        }
        else
        {
            attackQueued = true;
        }
    }

    private void PlayAttack(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    void StartCombo()
    {
        comboStep = 1;
        lastAttackTime = Time.time;
        PlayAttack("Attack1");
    }

    void ContinueCombo()
    {
        attackQueued = false;

        comboStep++;
        if (comboStep > 3) comboStep = 1;

        lastAttackTime = Time.time;

        switch (comboStep)
        {
            case 2: PlayAttack("Attack2"); break;
            case 3: PlayAttack("Attack3"); break;
        }
    }

    public void OnAttack3End()
    {
        ResetCombo();

        animator.CrossFade("Mario_Idle", 0.05f);
    }

    void ResetCombo()
    {
        comboStep = 0;
        attackQueued = false;
        canChain = false;

        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
        animator.ResetTrigger("ExitAttack");
    }

    public void EnableAttackHitbox()
    {
        AttackHitbox.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        AttackHitbox.SetActive(false);
    }

    public void EnableComboWindow()
    {
        canChain = true;
        if (attackQueued)
        {
            ContinueCombo();
        }
    }

    public void DisableComboWindow()
    {
        canChain = false;

        if (!attackQueued)
        {
            StartCoroutine(ExitAttackNextFrame());
        }
    }

    public bool IsFalling()
    {
        return rb.linearVelocity.y < -0.1f;
    }

    public int GetDamage()
    {
        return isPoweredUp
            ? Mathf.RoundToInt(Damage * MushroomMultiplier)
            : Damage;
    }

    public void SetPoweredUp(bool value)
    {
        isPoweredUp = value;
    }

    IEnumerator ExitAttackNextFrame()
    {
        yield return null;

        if (!attackQueued && !canChain)
        {
            animator.SetTrigger("ExitAttack");
        }
    }
}