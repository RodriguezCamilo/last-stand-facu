using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootingCooldown = 2f;
    [SerializeField] private float shootingDistance = 10f;
    [SerializeField] private Transform shootPoint;

    private bool canShoot = true;

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Sigue al jugador hasta cierta distancia, luego se queda quieto y dispara
            float distance = Vector3.Distance(transform.position, player.position);
            
            Vector3 look = player.position;
            look.y = transform.position.y;
            transform.LookAt(look);

            if (distance > shootingDistance)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * speed * Time.fixedDeltaTime;
            }
            else
            {
                if (canShoot)
                {
                    Shoot();
                }
            }
        }

        // Copio las mismas correcciones de físicas que usa Enemy

        // Para que no salgan disparados verticalmente cuando spawnean
        Vector3 v = rb.velocity;
        if (v.y > 0) v.y = 0;
        rb.velocity = v;

        // Lo mismo pero horizontal
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > speed)
        {
            flatVelocity = flatVelocity.normalized * speed;
            rb.velocity = new Vector3(flatVelocity.x, 0f, flatVelocity.z);
        }
    }

    private void Shoot()
    {
        // Basicamente el mismo codigo de disparo del jugador pero la rotacion esta indicada por la posicion del player e ignoramos altura
        if (projectilePrefab != null && shootPoint != null)
        {
            Vector3 direction = player.position - shootPoint.position;
            direction.y = 0f;

            Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(direction));
        }

        canShoot = false;
        Invoke(nameof(ResetShoot), shootingCooldown);
    }

    private void ResetShoot()
    {
        canShoot = true;
    }
}
