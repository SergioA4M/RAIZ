using System;
using System.Collections;
using UnityEngine;

// Vida genérica: la usan tanto el espíritu como las plagas.
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    // Segundos sin recibir daño después de un golpe (0 = sin protección).
    [SerializeField] private float invulnerableTime = 0f;

    // Las plagas desaparecen al morir; el espíritu no (lo gestiona PlayerDeath).
    [SerializeField] private bool destroyOnDeath = true;

    public int CurrentHealth { get; private set; }

    // Evento: otros scripts se "suscriben" para enterarse de la muerte.
    public event Action Died;

    private SpriteRenderer sprite;
    private Color originalColor;
    private float invulnerableUntil;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            originalColor = sprite.color;
        }
    }

    public void TakeDamage(int amount)
    {
        // Ya muerto o todavía protegido: ignoramos el golpe.
        if (CurrentHealth <= 0 || Time.time < invulnerableUntil) return;

        CurrentHealth -= amount;
        invulnerableUntil = Time.time + invulnerableTime;

        if (sprite != null)
        {
            StartCoroutine(Flash());
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    // Corrutina: una función que puede "esperar" sin congelar el juego.
    // Pone el sprite en blanco un instante para que se note el golpe.
    private IEnumerator Flash()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
    }

    private void Die()
    {
        Died?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}
