using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        // La bala tiene un alcance limite, de paso aprovechamos para sacarla de la escena
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Para seguir los patrones de disenio planteados en programacion 2 aproveche a usar la interfaz
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
