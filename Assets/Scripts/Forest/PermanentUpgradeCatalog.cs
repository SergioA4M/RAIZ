using System.Collections.Generic;
using UnityEngine;

// Lista de todas las mejoras permanentes del juego, en un solo archivo.
// La usan la tienda del bosque (para venderlas) y el espíritu (para aplicarlas).
// Así, si añades una mejora nueva, solo la pones aquí: "una única fuente de verdad".
[CreateAssetMenu(fileName = "CatalogoBosque", menuName = "Raiz/Catálogo de mejoras permanentes")]
public class PermanentUpgradeCatalog : ScriptableObject
{
    public List<PermanentUpgrade> upgrades = new List<PermanentUpgrade>();
}
