using UnityEngine;

// Temblor de pantalla basado en "trauma" (una técnica muy usada en juegos):
// cada golpe suma trauma (de 0 a 1), el trauma baja solo con el tiempo,
// y la cámara tiembla más cuanto más trauma haya.
// No mueve la cámara él mismo: calcula un desplazamiento (Offset) que usa CameraFollow.
public class CameraShake : MonoBehaviour
{
    // Patrón "singleton": una forma sencilla de que cualquier script encuentre
    // este componente sin tener que arrastrarlo al Inspector.
    // Solo tiene sentido cuando hay UNO en la escena (la cámara).
    public static CameraShake Instance { get; private set; }

    [SerializeField] private float maxOffset = 0.4f;     // desplazamiento máximo en unidades
    [SerializeField] private float recoverySpeed = 1.5f; // trauma que se pierde por segundo

    public Vector3 Offset { get; private set; }

    private float trauma;

    private void Awake()
    {
        Instance = this;
    }

    // Atajo para llamarlo desde cualquier sitio: CameraShake.Add(0.3f);
    // "static" = se llama sobre la clase, no sobre un objeto concreto.
    public static void Add(float amount)
    {
        if (Instance != null)
        {
            Instance.trauma = Mathf.Clamp01(Instance.trauma + amount);
        }
    }

    private void Update()
    {
        // unscaledDeltaTime: el temblor sigue aunque el tiempo del juego esté parado (hit stop).
        trauma = Mathf.Max(trauma - recoverySpeed * Time.unscaledDeltaTime, 0f);

        // Elevar al cuadrado hace que los golpes pequeños tiemblen poco
        // y los grandes mucho: se siente más natural.
        float intensity = trauma * trauma;
        Offset = (Vector3)(Random.insideUnitCircle * maxOffset * intensity);
    }
}
