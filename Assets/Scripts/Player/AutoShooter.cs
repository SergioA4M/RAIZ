using UnityEngine;

// Dispara automáticamente hojas hacia la plaga más cercana.
public class AutoShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float range = 8f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        EnemyChase target = FindNearestEnemy();
        if (target == null) return;

        timer = 0f;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Launch(direction);
    }

    private EnemyChase FindNearestEnemy()
    {
        EnemyChase nearest = null;

        // Comparamos distancias al cuadrado (sqrMagnitude): es más rápido
        // que calcular la raíz cuadrada y el resultado de la comparación es el mismo.
        float bestDistance = range * range;

        foreach (EnemyChase enemy in EnemyChase.All)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
