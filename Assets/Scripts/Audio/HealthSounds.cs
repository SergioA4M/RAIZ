using UnityEngine;

// Sonidos al recibir daño y al morir, para cualquier objeto con Health.
[RequireComponent(typeof(Health))]
public class HealthSounds : MonoBehaviour
{
    [SerializeField] private AudioClip damagedSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private void Awake()
    {
        Health health = GetComponent<Health>();
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    private void OnDamaged(int amount)
    {
        SoundManager.Play(damagedSound, volume);
    }

    private void OnDied()
    {
        SoundManager.Play(deathSound, volume);
    }
}
