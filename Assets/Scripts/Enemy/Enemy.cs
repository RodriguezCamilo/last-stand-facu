using UnityEngine;

public class Enemy : Character, IDamageable
{
    protected Transform player;
    protected Rigidbody rb;

    [SerializeField] protected float speed = 3f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    // --- NUEVO ---
    [SerializeField] private float turnSpeed = 10f;   // Velocidad de giro

    private bool canAttack = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Mover hacia el jugador
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.fixedDeltaTime;
            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
            }
        }

        //Consulte a la IA, tenia idea de como solucionarlo, limitando la velocidad vertical y horizontal, pero no soy el mejor matematico y todavia no tengo tanta experiencia con vectores y el rigidbody
        //Para que no salgan disparados verticalmente cuando spawnean
        Vector3 v = rb.velocity;
        rb.velocity = new Vector3(v.x, 0f, v.z);

        // Lo mismo pero horizontal
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > speed)
        {
            flatVelocity = flatVelocity.normalized * speed;
            rb.velocity = new Vector3(flatVelocity.x, 0f, flatVelocity.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canAttack)
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
                canAttack = false;
                Invoke(nameof(ResetAttack), attackCooldown);
            }
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    public void ReceiveDamage(float damage)
    {
        TakeDamage(damage);
    }

    protected override void Die()
    {
        Debug.Log("Enemigo murio");
        WaveSpawner spawner = FindObjectOfType<WaveSpawner>();
        if (spawner != null)
        {
            spawner.EnemyDied();
        }
        Destroy(gameObject, 0.2f);
    }
}
