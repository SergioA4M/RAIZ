using UnityEngine;

// La cámara sigue al objetivo (el espíritu) con un movimiento suave.
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 8f;

    // LateUpdate se ejecuta después de todos los Update:
    // así la cámara se mueve cuando el jugador ya se ha movido.
    private void LateUpdate()
    {
        if (target == null) return;

        // Mantenemos la z de la cámara; si no, se pondría encima del jugador y no vería nada.
        Vector3 goal = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Lerp acerca poco a poco la cámara hacia el objetivo.
        transform.position = Vector3.Lerp(transform.position, goal, smoothSpeed * Time.deltaTime);
    }
}
