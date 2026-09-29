using UnityEngine;

// Pequeña flecha pegada al espíritu que señala hacia dónde va a disparar.
// Va en un objeto HIJO del espíritu (así se mueve con él automáticamente).
public class DirectionIndicator : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;

    [Tooltip("Distancia al centro del espíritu, en unidades locales (0.5 = justo en el borde del círculo)")]
    [SerializeField] private float distance = 0.75f;

    private void LateUpdate()
    {
        Vector2 direction = movement.FacingDirection;

        // localPosition = posición relativa al padre (el espíritu).
        transform.localPosition = direction * distance;

        // Giramos la flecha para que apunte en esa dirección.
        // El sprite Triangle apunta hacia arriba (90°), por eso restamos 90.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
