using System.Collections.Generic;
using UnityEngine;

// Hace que el enemigo persiga al espíritu en línea recta.
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    // Lista compartida con todos los enemigos vivos.
    // "static" = pertenece a la clase, no a cada enemigo: solo hay una lista.
    // La usa el AutoShooter para encontrar al más cercano sin buscar por toda la escena.
    public static readonly List<EnemyChase> All = new List<EnemyChase>();

    [SerializeField] private float speed = 2f;

    private Rigidbody2D rb;
    private Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // OnEnable / OnDisable: al aparecer se apunta a la lista, al morir se borra.
    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    // Start se ejecuta justo antes del primer frame del objeto.
    private void Start()
    {
        // Busca en la escena el objeto que tiene el script PlayerMovement.
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // Dirección hacia el jugador, con longitud 1 (normalized),
        // para que la velocidad no dependa de lo lejos que esté.
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.linearVelocity = direction * speed;
    }
}
