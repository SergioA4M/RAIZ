using UnityEngine;
using UnityEngine.InputSystem;

// Mueve al espíritu con WASD / flechas usando la física 2D de Unity.
// Necesita un Rigidbody2D en el mismo objeto (Unity lo añade solo si falta).
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // [SerializeField] hace que la variable aparezca en el Inspector,
    // así puedes ajustar la velocidad sin tocar el código.
    [SerializeField] private float speed = 5f;

    // Hacia dónde mira el espíritu: la última dirección en la que se movió,
    // ajustada a 8 direcciones. Al empezar, mira a la derecha.
    // La usa AutoShooter para disparar y DirectionIndicator para dibujar la flecha.
    public Vector2 FacingDirection { get; private set; } = Vector2.right;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 moveInput;

    // Awake se ejecuta una sola vez, al crearse el objeto.
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // "Move" es una acción que ya viene definida en
        // Assets/Settings/InputSystem_Actions (WASD, flechas y mando).
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update se ejecuta cada frame: aquí leemos el teclado.
    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        // Solo cambiamos la dirección si se está pulsando algo:
        // al soltar las teclas, el espíritu sigue mirando hacia el último lado.
        if (moveInput.sqrMagnitude > 0.01f)
        {
            FacingDirection = SnapToEightDirections(moveInput);
        }
    }

    // Redondea cualquier dirección a la más cercana de las 8:
    // derecha, arriba-derecha, arriba, arriba-izquierda, izquierda...
    // Con teclado ya salen 8 direcciones, pero con el joystick de un mando
    // podría salir cualquier ángulo (por ejemplo 37°).
    private static Vector2 SnapToEightDirections(Vector2 direction)
    {
        // Atan2 da el ángulo de un vector (en radianes): derecha = 0°, arriba = 90°...
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Lo redondeamos al múltiplo de 45° más cercano (0, 45, 90, 135...).
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;

        // Y volvemos de ángulo a vector con coseno y seno.
        float radians = snappedAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    // FixedUpdate se ejecuta a ritmo fijo: aquí movemos la física.
    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }

    // Mejora: percent = 0.1 significa "+10 % de velocidad".
    public void IncreaseSpeed(float percent)
    {
        speed *= 1f + percent;
    }
}
