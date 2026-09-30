using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Cuando el espíritu muere: desaparece y deja de moverse y disparar.
// Lo que pasa después (pantalla de fin de partida) lo decide GameOverScreen.
// Si no hay pantalla de fin de partida, se puede marcar autoRestart para reiniciar solo.
[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    [Tooltip("Reiniciar la partida sola al morir (solo si no se usa GameOverScreen)")]
    [SerializeField] private bool autoRestart = false;
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
        // GetComponentsInChildren: el sprite del espíritu y los de sus hijos (la flecha de dirección).
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.enabled = false;
        }

        if (autoRestart)
        {
            StartCoroutine(RestartAfterDelay());
        }
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);

        // Recarga la escena actual: todo vuelve a su estado inicial.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
