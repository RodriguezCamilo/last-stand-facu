using UnityEngine;

public class Player : Character, IDamageable
{
    [SerializeField] LayerMask m_floorMask;
    [SerializeField] float m_speed = 6f;
    [SerializeField] Transform m_bulletSpawner;
    [SerializeField] GameObject m_bullet;

    //Dasheito salvaje de chill 🤙
    [SerializeField] private float dashForce = 50f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private AudioClip dashClip;
    private bool isDashing = false;
    private bool canDash = true;

    private float lastDashTime = -999f;

    // Sonido
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip stepClip;
    [SerializeField] private float stepInterval = 0.4f;
    private AudioSource shootAudioSource;


    private float stepTimer;
    private bool isMoving;
    private bool isDead = false;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    Vector3 m_movement;
    Rigidbody m_playerRigidbody;
    float m_camRayLenght = 100f;
    float m_horz = 0f;
    float m_vert = 0f;
    float m_coldDown = 0.2f;
    float m_coldDownTimer;
    Ray m_camRay;

    Animator animator;

    RaycastHit m_floorHit;
    Vector3 m_playerToMouse;

    protected override void Awake()
    {
        base.Awake();
        m_playerRigidbody = GetComponent<Rigidbody>();
        shootAudioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (isDead) return;
        // Movi los imputs a aca porque fixedupdate no me los toma bien
        m_coldDownTimer += Time.deltaTime;

        m_horz = Input.GetAxisRaw("Horizontal");
        m_vert = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            Dash();
        }

        if (Input.GetButton("Fire1") && m_coldDownTimer > m_coldDown)
        {
            Shoot();
        }

        isMoving = m_horz != 0 || m_vert != 0;
        animator.SetFloat("Speed", new Vector3(m_horz, 0, m_vert).magnitude);

        // Sonido de pasos
        if (isMoving)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                PlayStepSound();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        Move();
        Turning();
    }

    void Move()
    {
        // Este tambien jeje
        if (isDashing)
        {
            return;
        }
        m_movement.Set(m_horz, 0f, m_vert);
        m_movement = m_movement.normalized * m_speed;
        m_playerRigidbody.MovePosition(transform.position + m_movement * Time.fixedDeltaTime);
    }

    private void Dash()
    {
        // Checkeo que el personaje ya se este moviendo hacia algun lado
        Vector3 dashDirection = new Vector3(m_horz, 0f, m_vert).normalized;

        if (dashDirection.magnitude == 0)
        {
            return;
        }

        // Impulzamos
        m_playerRigidbody.velocity = Vector3.zero;
        m_playerRigidbody.AddForce(dashDirection * dashForce, ForceMode.Impulse);

        isDashing = true;
        if (dashClip != null && shootAudioSource != null)
        {
            shootAudioSource.PlayOneShot(dashClip);
        }

        // Cooldown
        canDash = false;
        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
        lastDashTime = Time.time;
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private void EndDash()
    {
        isDashing = false;
        m_playerRigidbody.velocity = Vector3.zero;
    }


    void Turning()
    {
        m_camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(m_camRay, out m_floorHit, m_camRayLenght, m_floorMask))
        {
            m_playerToMouse = m_floorHit.point - transform.position;
            m_playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(m_playerToMouse, Vector3.up);
            m_playerRigidbody.MoveRotation(newRotation);
        }
    }

    void Shoot()
    {
        m_coldDownTimer = 0;
        Debug.Log("Shoot");
        animator.SetTrigger("Shoot");
        Instantiate(m_bullet, m_bulletSpawner.position, m_bulletSpawner.rotation);
        if (shootClip != null && shootAudioSource != null)
        {
            shootAudioSource.PlayOneShot(shootClip);
        }


    }

    public float DashCooldown()
    {
        return Mathf.Clamp01((Time.time - lastDashTime) / dashCooldown);
    }


    private void PlayStepSound()
    {
        if (stepClip != null && shootAudioSource != null && !isDashing)
        {
            shootAudioSource.PlayOneShot(stepClip);
        }
    }


    protected override void Die()
    {
        Debug.Log("Player murio");
        isDead = true;
        animator.SetBool("Death", isDead);
        m_playerRigidbody.velocity = Vector3.zero;
        m_playerRigidbody.isKinematic = true;
        // gameObject.SetActive(false);
        GameManager.Instance.PlayerDied();
    }

    public void ResetPlayer()
    {
        animator.SetBool("Death", false);
        animator.Play("Idle");
        currentHealth = maxHealth;
        isDead = false;
        m_playerRigidbody.isKinematic = false;
    }

}
