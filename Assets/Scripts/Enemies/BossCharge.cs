using System.Collections;
using UnityEngine;

// Ataque especial del jefe: cada cierto tiempo se para, cambia de color
// (aviso para que el jugador reaccione) y embiste a toda velocidad.
// Es una pequeña "máquina de estados": perseguir → avisar → embestir → perseguir…
[RequireComponent(typeof(Rigidbody2D), typeof(EnemyChase))]
public class BossCharge : MonoBehaviour
{
    [SerializeField] private float chargeCooldown = 4f;
    [SerializeField] private float windUpTime = 0.7f;
    [SerializeField] private float chargeSpeed = 12f;
    [SerializeField] private float chargeDuration = 0.5f;
    [SerializeField] private Color windUpColor = Color.yellow;

    private Rigidbody2D rb;
    private EnemyChase chase;
    private SpriteRenderer sprite;
    private Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        chase = GetComponent<EnemyChase>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            target = player.transform;
        }

        StartCoroutine(ChargeLoop());
    }

    private IEnumerator ChargeLoop()
    {
        // Un bucle infinito dentro de una corrutina no congela el juego:
        // cada "yield return" le devuelve el control a Unity.
        while (target != null)
        {
            // 1. Perseguir normalmente.
            yield return new WaitForSeconds(chargeCooldown);

            // 2. Aviso: se para y cambia de color.
            chase.CanMove = false;
            rb.linearVelocity = Vector2.zero;
            Color originalColor = sprite.color;
            sprite.color = windUpColor;
            yield return new WaitForSeconds(windUpTime);
            sprite.color = originalColor;

            if (target == null) yield break;

            // 3. Embestida en línea recta hacia donde estaba el jugador.
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;
            rb.linearVelocity = direction * chargeSpeed;
            yield return new WaitForSeconds(chargeDuration);

            // 4. Vuelta a perseguir.
            chase.CanMove = true;
        }
    }
}
