using UnityEngine;

// La cámara sigue al objetivo (el espíritu) con un movimiento suave.
// Si en el mismo objeto hay un CameraShake, le suma su temblor.
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 8f;

    // Guardamos la posición "de seguimiento" aparte, sin el temblor.
    // Si usáramos transform.position, el temblor se iría acumulando.
    private Vector3 followPosition;
    private CameraShake shake;

    private void Awake()
    {
        followPosition = transform.position;
        shake = GetComponent<CameraShake>();
    }

    // LateUpdate se ejecuta después de todos los Update:
    // así la cámara se mueve cuando el jugador ya se ha movido.
    private void LateUpdate()
    {
        if (target == null) return;

        // Mantenemos la z de la cámara; si no, se pondría encima del jugador y no vería nada.
        Vector3 goal = new Vector3(target.position.x, target.position.y, followPosition.z);

        // Lerp acerca poco a poco la cámara hacia el objetivo.
        followPosition = Vector3.Lerp(followPosition, goal, smoothSpeed * Time.deltaTime);

        Vector3 shakeOffset = shake != null ? shake.Offset : Vector3.zero;
        transform.position = followPosition + shakeOffset;
    }
}
