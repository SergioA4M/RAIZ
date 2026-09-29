using UnityEngine;

// Al morir, deja caer un objeto (una semilla) donde estaba.
[RequireComponent(typeof(Health))]
public class DropOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;

    private void Awake()
    {
        GetComponent<Health>().Died += Drop;
    }

    private void Drop()
    {
        Instantiate(dropPrefab, transform.position, Quaternion.identity);
    }
}
