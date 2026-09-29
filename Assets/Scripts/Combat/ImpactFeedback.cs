using UnityEngine;

// Sensación de impacto configurable desde el Inspector:
// temblor de pantalla al recibir daño y al morir, y hit stop al morir.
// Se pone en cualquier objeto con Health (espíritu, plagas, jefe) y cada uno con sus valores.
[RequireComponent(typeof(Health))]
public class ImpactFeedback : MonoBehaviour
{
    [Tooltip("Temblor al recibir daño (0 = nada, 1 = máximo)")]
    [SerializeField, Range(0f, 1f)] private float shakeOnDamage = 0f;

    [Tooltip("Temblor al morir (0 = nada, 1 = máximo)")]
    [SerializeField, Range(0f, 1f)] private float shakeOnDeath = 0f;

    [Tooltip("Segundos de congelación al morir (0 = nada). Valores típicos: 0.05 a 0.15")]
    [SerializeField] private float hitStopOnDeath = 0f;

    private void Awake()
    {
        Health health = GetComponent<Health>();
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    private void OnDamaged(int amount)
    {
        if (shakeOnDamage > 0f) CameraShake.Add(shakeOnDamage);
    }

    private void OnDied()
    {
        if (shakeOnDeath > 0f) CameraShake.Add(shakeOnDeath);
        if (hitStopOnDeath > 0f) HitStop.Freeze(hitStopOnDeath);
    }
}
