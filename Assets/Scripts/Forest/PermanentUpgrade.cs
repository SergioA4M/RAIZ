using UnityEngine;

// Mejora permanente que se compra en el bosque con las semillas del banco.
// No repite la lógica del efecto: reutiliza un UpgradeData (las mejoras de la partida)
// y lo aplica una vez por cada nivel comprado al empezar cada partida.
[CreateAssetMenu(fileName = "NuevaMejoraPermanente", menuName = "Raiz/Mejora permanente")]
public class PermanentUpgrade : ScriptableObject
{
    [Tooltip("Identificador único para el guardado. ¡No lo cambies una vez publicado el juego o se perderá el progreso de esta mejora!")]
    public string id;

    public string title;
    [TextArea] public string description;

    [Tooltip("Efecto que se aplica una vez por nivel (una de las mejoras de Assets/Mejoras)")]
    public UpgradeData effect;

    public int maxLevel = 5;

    [Tooltip("Coste del nivel 1")]
    public int baseCost = 10;

    [Tooltip("Cuánto sube el coste en cada nivel")]
    public int costIncrease = 10;

    // Coste para pasar del nivel actual al siguiente: 10, 20, 30...
    public int GetCost(int currentLevel)
    {
        return baseCost + costIncrease * currentLevel;
    }
}
