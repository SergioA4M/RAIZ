using UnityEngine;
using UnityEngine.InputSystem;

// Dispara hojas. Dos opciones configurables en el Inspector:
// - Cuándo: solo (autoFire) o al pulsar la acción "Attack" (Espacio, clic izquierdo, Enter...).
// - Hacia dónde: a la plaga más cercana (autoAim) o hacia donde mira el espíritu (8 direcciones).
// (El nombre AutoShooter viene de la primera versión; cambiarlo obligaría a reconectar el componente.)
public class AutoShooter : MonoBehaviour
{
    [Tooltip("Marcado: dispara solo. Desmarcado: hay que pulsar Attack (mantener pulsado = ráfaga)")]
    [SerializeField] private bool autoFire = false;

    [Tooltip("Marcado: apunta sola a la plaga más cercana. Desmarcado: dispara hacia donde mira el espíritu")]
    [SerializeField] private bool autoAim = false;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float fireInterval = 0.5f;
    [Tooltip("Distancia máxima para buscar enemigos (solo con Auto Aim)")]
    [SerializeField] private float range = 8f;
    [SerializeField] private int damage = 1;

    // Hojas por disparo. Con más de una, salen en abanico.
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 12f;

    [Header("Sonido")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.4f;

    // Para que otros scripts (por ejemplo, la herramienta de capturas) puedan cambiar el modo.
    public bool AutoFire { get => autoFire; set => autoFire = value; }
    public bool AutoAim { get => autoAim; set => autoAim = value; }

    private float timer;
    private InputAction attackAction;
    private PlayerMovement movement;

    private void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // En pausa (menú de mejoras) no se dispara: el clic para elegir mejora
        // también cuenta como "Attack".
        if (Time.timeScale == 0f) return;

        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        // IsPressed = la tecla está pulsada ahora mismo. Mantenerla dispara en ráfaga,
        // al ritmo de fireInterval (así la mejora de cadencia sigue sirviendo).
        if (!autoFire && !attackAction.IsPressed()) return;

        Vector2 direction;
        if (autoAim)
        {
            EnemyChase target = FindNearestEnemy();
            if (target == null) return; // nadie a tiro: no gastamos el disparo
            direction = (target.transform.position - transform.position).normalized;
        }
        else
        {
            direction = movement.FacingDirection;
        }

        timer = 0f;

        // Repartimos las hojas en abanico, centrado en la dirección de disparo.
        // Con 3 hojas y 12°: -12°, 0°, +12°.
        float firstAngle = -spreadAngle * (projectileCount - 1) / 2f;
        for (int i = 0; i < projectileCount; i++)
        {
            // Multiplicar un Quaternion por un vector lo gira ese ángulo.
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, firstAngle + spreadAngle * i) * direction;

            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Launch(shotDirection, damage);
        }

        // Un solo sonido por ráfaga, aunque salgan varias hojas.
        SoundManager.Play(shootSound, shootVolume);
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
