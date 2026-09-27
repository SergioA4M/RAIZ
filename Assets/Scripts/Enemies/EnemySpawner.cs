using UnityEngine;

// Crea enemigos cada cierto tiempo en un círculo alrededor del jugador,
// fuera de la pantalla, para que "entren" desde los bordes.
public class EnemySpawner : MonoBehaviour
{
    // El prefab es la "plantilla" del enemigo que vamos a copiar.
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnDistance = 12f;

    private Transform player;
    private float timer;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        // Time.deltaTime = segundos que han pasado desde el frame anterior.
        // Sumándolo tenemos un cronómetro.
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (player == null) return;

        // Una dirección al azar (x, y) de longitud 1.
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 position = player.position + (Vector3)(direction * spawnDistance);

        // Instantiate crea una copia del prefab en esa posición.
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }
}
