using UnityEngine;

// Al morir, deja caer uno o varios objetos (semillas) donde estaba.
[RequireComponent(typeof(Health))]
public class DropOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private int amount = 1;

    // Si suelta varios, los reparte en un círculo de este radio.
    [SerializeField] private float scatterRadius = 1f;

    private void Awake()
    {
        GetComponent<Health>().Died += Drop;
    }

    private void Drop()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 offset = amount > 1 ? (Vector3)(Random.insideUnitCircle * scatterRadius) : Vector3.zero;
            Instantiate(dropPrefab, transform.position + offset, Quaternion.identity);
        }
    }
}
