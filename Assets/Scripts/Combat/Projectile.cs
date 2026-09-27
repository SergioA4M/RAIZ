using UnityEngine;

// Hoja afilada que vuela en línea recta y daña a la primera plaga que toca.
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 3f;

    // La llama el AutoShooter justo después de crear la hoja.
    public void Launch(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;

        // Si no choca con nada, se destruye sola a los pocos segundos.
        Destroy(gameObject, lifetime);
    }

    // Se llama cuando entra en contacto con otro collider
    // (el collider de la hoja tiene "Is Trigger" activado: atraviesa, no empuja).
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo nos interesan las plagas (tienen EnemyChase) que tengan vida.
        if (other.TryGetComponent(out EnemyChase _) && other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
