using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Dibuja el bosque: un árbol por cada nivel de mejora comprado.
// Los árboles salen siempre en los mismos sitios gracias a un generador
// de números aleatorios con "semilla" fija (curiosamente, también se llama semilla).
public class ForestGrower : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;

    [Tooltip("Zona donde pueden crecer árboles (centro y tamaño, en unidades del mundo)")]
    [SerializeField] private Vector2 areaCenter = new Vector2(3.5f, -0.5f);
    [SerializeField] private Vector2 areaSize = new Vector2(8f, 6f);

    [SerializeField] private int maxTrees = 60;
    [SerializeField] private float minDistanceBetweenTrees = 0.7f;
    [SerializeField] private int randomSeed = 2026;

    private readonly List<Vector2> treePositions = new List<Vector2>();
    private int plantedTrees;

    private void Awake()
    {
        GeneratePositions();
    }

    private void Start()
    {
        // Al entrar en el bosque, los árboles que ya tenías aparecen sin animación.
        int treesToShow = SaveSystem.Data.TotalUpgradeLevels();
        for (int i = 0; i < treesToShow; i++)
        {
            PlantTree(animate: false);
        }
    }

    // La llama la tienda al comprar una mejora: un árbol nuevo que crece.
    public void GrowNewTree()
    {
        PlantTree(animate: true);
    }

    private void GeneratePositions()
    {
        // System.Random con semilla fija: siempre genera la MISMA secuencia de números.
        // (UnityEngine.Random es global y lo usan otros scripts; este es solo nuestro.)
        System.Random rng = new System.Random(randomSeed);

        int attempts = 0;
        while (treePositions.Count < maxTrees && attempts < maxTrees * 30)
        {
            attempts++;
            float x = areaCenter.x + ((float)rng.NextDouble() - 0.5f) * areaSize.x;
            float y = areaCenter.y + ((float)rng.NextDouble() - 0.5f) * areaSize.y;
            Vector2 candidate = new Vector2(x, y);

            if (IsFarFromOtherTrees(candidate))
            {
                treePositions.Add(candidate);
            }
        }
    }

    private bool IsFarFromOtherTrees(Vector2 candidate)
    {
        foreach (Vector2 position in treePositions)
        {
            if (Vector2.Distance(position, candidate) < minDistanceBetweenTrees) return false;
        }
        return true;
    }

    private void PlantTree(bool animate)
    {
        if (plantedTrees >= treePositions.Count) return; // bosque lleno

        Vector2 position = treePositions[plantedTrees];
        plantedTrees++;

        GameObject tree = Instantiate(treePrefab, position, Quaternion.identity, transform);

        // Los árboles de más abajo en la pantalla están "más cerca": se dibujan delante.
        // La Sorting Group dibuja tronco y copa juntos, como un solo objeto.
        if (tree.TryGetComponent(out SortingGroup sortingGroup))
        {
            sortingGroup.sortingOrder = -Mathf.RoundToInt(position.y * 100f);
        }

        if (animate)
        {
            StartCoroutine(GrowAnimation(tree.transform));
        }
    }

    // Crece desde 0 hasta su tamaño, pasándose un poco y volviendo (efecto "muelle").
    private IEnumerator GrowAnimation(Transform tree)
    {
        Vector3 finalScale = tree.localScale;
        const float duration = 0.6f;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float progress = t / duration;
            // Curva que sube hasta ~1.15 y se asienta en 1.
            float scale = 1f + Mathf.Sin(progress * Mathf.PI) * 0.15f;
            tree.localScale = finalScale * Mathf.Min(progress * 2f, 1f) * scale;
            yield return null;
        }
        tree.localScale = finalScale;
    }
}
