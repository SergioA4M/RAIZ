using UnityEngine;

// Semilla que sueltan las plagas. Cuando el espíritu se acerca,
// sale volando hacia él (efecto imán) y le da experiencia.
public class SeedPickup : MonoBehaviour
{
    [SerializeField] private int xpValue = 1;
    [SerializeField] private float magnetRadius = 2.5f;
    [SerializeField] private float magnetSpeed = 12f;

    private Transform player;
    private bool attracted;

    private void Start()
    {
        PlayerExperience experience = FindAnyObjectByType<PlayerExperience>();
        if (experience != null)
        {
            player = experience.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Una vez atraída, ya no se suelta aunque el jugador se aleje.
        if (!attracted && (player.position - transform.position).sqrMagnitude < magnetRadius * magnetRadius)
        {
            attracted = true;
        }

        if (attracted)
        {
            // MoveTowards avanza hacia el objetivo sin pasarse.
            transform.position = Vector3.MoveTowards(transform.position, player.position, magnetSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerExperience experience))
        {
            experience.AddXP(xpValue);
            Destroy(gameObject);
        }
    }
}
