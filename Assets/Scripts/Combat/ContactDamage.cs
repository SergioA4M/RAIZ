using UnityEngine;

// Hace daño al espíritu mientras la plaga lo esté tocando.
// La protección tras cada golpe (invulnerableTime en Health) evita
// que pierda toda la vida en un segundo.
public class ContactDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    // Se llama en cada paso de física mientras dos colliders se tocan.
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            collision.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
