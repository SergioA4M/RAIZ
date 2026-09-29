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
