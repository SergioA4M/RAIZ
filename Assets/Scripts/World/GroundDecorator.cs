using System.Collections.Generic;
using UnityEngine;

// Reparte decoración (hierba, flores, piedras) por el suelo alrededor del jugador.
// Truco para un mundo "infinito" con pocos objetos: cuando una decoración se queda
// demasiado lejos por un lado, se teletransporta al lado contrario.
// Así siempre hay decoración cerca, aunque el jugador camine sin parar.
public class GroundDecorator : MonoBehaviour
{
    [SerializeField] private GameObject[] decorationPrefabs;
    [SerializeField] private int count = 150;

    [Tooltip("Lado del cuadrado donde se reparte la decoración. Debe ser mayor que lo que ve la cámara.")]
    [SerializeField] private float areaSize = 30f;

    [SerializeField] private float minScale = 0.7f;
    [SerializeField] private float maxScale = 1.3f;

    private readonly List<Transform> decorations = new List<Transform>();
    private Transform player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;
        float half = areaSize / 2f;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = decorationPrefabs[Random.Range(0, decorationPrefabs.Length)];
            Vector3 position = player.position + new Vector3(Random.Range(-half, half), Random.Range(-half, half), 0f);

            // El 4º parámetro (transform) la hace hija de este objeto:
            // la Hierarchy queda ordenada en vez de llenarse de 150 objetos sueltos.
            GameObject decoration = Instantiate(prefab, position, Quaternion.identity, transform);

            // Variedad: tamaño al azar y volteada a izquierda o derecha.
            decoration.transform.localScale *= Random.Range(minScale, maxScale);
            if (decoration.TryGetComponent(out SpriteRenderer sprite))
            {
                sprite.flipX = Random.value < 0.5f;
                sprite.sortingOrder = -10; // siempre por debajo de personajes y enemigos
            }

            decorations.Add(decoration.transform);
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        float half = areaSize / 2f;

        foreach (Transform decoration in decorations)
        {
            Vector3 offset = decoration.position - player.position;

            // Si se ha quedado fuera del cuadrado por un lado, pasa al lado opuesto.
            if (offset.x > half) decoration.position -= new Vector3(areaSize, 0f, 0f);
            else if (offset.x < -half) decoration.position += new Vector3(areaSize, 0f, 0f);

            if (offset.y > half) decoration.position -= new Vector3(0f, areaSize, 0f);
            else if (offset.y < -half) decoration.position += new Vector3(0f, areaSize, 0f);
        }
    }
}
