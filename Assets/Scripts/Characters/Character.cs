using UnityEngine;

// Clase base para cualquier tipo de personaje, hereda de MonoBehaviour, para seguir patrones de dise;o y estructura de proyecto

public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    public float maxHealth = 100f;
    protected float currentHealth;

    [Header("Audio")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip deathClip;
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        Debug.Log(currentHealth);

        if (hitClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitClip);
        }

        if (currentHealth <= 0)
        {
            if (deathClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(deathClip);
            }
            Die();
        }
    }

    protected abstract void Die();
}
