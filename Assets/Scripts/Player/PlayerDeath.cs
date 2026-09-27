using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Cuando el espíritu muere: desaparece, deja de moverse y disparar,
// y a los pocos segundos la partida vuelve a empezar.
[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float restartDelay = 2f;

    private void Awake()
    {
        // Nos suscribimos al evento Died de Health.
        GetComponent<Health>().Died += OnDied;
    }

    private void OnDied()
    {
        if (TryGetComponent(out PlayerMovement movement)) movement.enabled = false;
        if (TryGetComponent(out AutoShooter shooter)) shooter.enabled = false;
        if (TryGetComponent(out Rigidbody2D rb)) rb.linearVelocity = Vector2.zero;
        if (TryGetComponent(out SpriteRenderer sprite)) sprite.enabled = false;

        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);

        // Recarga la escena actual: todo vuelve a su estado inicial.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
