using UnityEngine;

// Dispara automáticamente hojas hacia la plaga más cercana.
public class AutoShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float range = 8f;
    [SerializeField] private int damage = 1;

    // Hojas por disparo. Con más de una, salen en abanico.
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 12f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        EnemyChase target = FindNearestEnemy();
        if (target == null) return;

        timer = 0f;

        Vector2 direction = (target.transform.position - transform.position).normalized;

        // Repartimos las hojas en abanico, centrado en la dirección del enemigo.
        // Con 3 hojas y 12°: -12°, 0°, +12°.
        float firstAngle = -spreadAngle * (projectileCount - 1) / 2f;
        for (int i = 0; i < projectileCount; i++)
        {
            // Multiplicar un Quaternion por un vector lo gira ese ángulo.
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, firstAngle + spreadAngle * i) * direction;

            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Launch(shotDirection, damage);
        }
    }

    // --- Mejoras (las llama UpgradeData) ---

    // percent = 0.2 significa "+20 % de disparos por segundo".
    public void IncreaseFireRate(float percent)
    {
        fireInterval /= 1f + percent;
    }

    public void AddDamage(int amount)
    {
        damage += amount;
    }

    public void AddProjectiles(int amount)
    {
        projectileCount += amount;
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
