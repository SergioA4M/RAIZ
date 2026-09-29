using System;
using System.Collections.Generic;
using UnityEngine;

// Una "regla de aparición": qué enemigo sale, desde qué segundo, hasta cuándo y cada cuánto.
// [Serializable] hace que Unity pueda mostrarla y editarla en el Inspector dentro de una lista.
[Serializable]
public class SpawnRule
{
    public string name;
    public GameObject enemyPrefab;

    [Tooltip("Segundo de partida en el que empieza a aparecer")]
    public float startTime;

    [Tooltip("Segundo en el que deja de aparecer (0 = no para nunca)")]
    public float endTime;

    [Tooltip("Segundos entre un enemigo y el siguiente")]
    public float spawnInterval = 1f;

    // Cronómetro propio de cada regla. [HideInInspector]: no hace falta verlo.
    [HideInInspector] public float timer;
}

// Crea enemigos alrededor del jugador, fuera de la pantalla, siguiendo una lista de reglas.
// Como varias reglas pueden estar activas a la vez, la dificultad sube con el tiempo.
// Al llegar a "bossTime" aparece el jefe (una sola vez).
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnRule> rules = new List<SpawnRule>();
    [SerializeField] private float spawnDistance = 12f;

    [Header("Jefe")]
    [SerializeField] private GameObject bossPrefab;
    [Tooltip("Segundo en el que aparece el jefe (300 = minuto 5)")]
    [SerializeField] private float bossTime = 300f;

    // Aviso para la interfaz: "ha aparecido el jefe, esta es su vida".
    public event Action<Health> BossSpawned;

    private Transform player;
    private bool bossHasSpawned;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        if (player == null) return;

        // Segundos desde que se cargó la escena. Se reinicia al morir (se recarga la escena)
        // y se para con la pausa del menú de mejoras.
        float gameTime = Time.timeSinceLevelLoad;

        foreach (SpawnRule rule in rules)
        {
            bool started = gameTime >= rule.startTime;
            bool finished = rule.endTime > 0f && gameTime >= rule.endTime;
            if (!started || finished) continue;

            rule.timer += Time.deltaTime;
            if (rule.timer >= rule.spawnInterval)
            {
                rule.timer = 0f;
                Spawn(rule.enemyPrefab);
            }
        }

        if (!bossHasSpawned && bossPrefab != null && gameTime >= bossTime)
        {
            bossHasSpawned = true;
            GameObject boss = Spawn(bossPrefab);
            BossSpawned?.Invoke(boss.GetComponent<Health>());
        }
    }

    private GameObject Spawn(GameObject prefab)
    {
        // Una dirección al azar (x, y) de longitud 1.
        Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 position = player.position + (Vector3)(direction * spawnDistance);

        // Instantiate crea una copia del prefab en esa posición.
        return Instantiate(prefab, position, Quaternion.identity);
    }
}
