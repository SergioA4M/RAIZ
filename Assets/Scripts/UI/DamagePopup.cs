using UnityEngine;

// Cada vez que el objeto recibe daño, crea un número flotante encima de él.
[RequireComponent(typeof(Health))]
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private DamageNumber numberPrefab;
    [SerializeField] private float heightOffset = 0.5f;

    private void Awake()
    {
        GetComponent<Health>().Damaged += ShowNumber;
    }

    private void ShowNumber(int amount)
    {
        // Un poco de desplazamiento horizontal al azar para que los números
        // de golpes seguidos no salgan unos encima de otros.
        Vector3 position = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), heightOffset, 0f);

        DamageNumber number = Instantiate(numberPrefab, position, Quaternion.identity);
        number.Setup(amount);
    }
}
