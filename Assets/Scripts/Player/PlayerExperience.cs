using System;
using UnityEngine;

// Lleva la cuenta de las semillas (experiencia) y del nivel del espíritu.
public class PlayerExperience : MonoBehaviour
{
    // Semillas necesarias para pasar del nivel 1 al 2.
    [SerializeField] private int firstLevelXP = 5;

    // Cada nivel pide un 40 % más de semillas que el anterior.
    [SerializeField] private float xpGrowth = 1.4f;

    public int Level { get; private set; } = 1;
    public int CurrentXP { get; private set; }
    public int XPToNextLevel { get; private set; }

    // Total de semillas recogidas en esta partida (no se gastan al subir de nivel).
    // Al morir se guardan en el "banco" del bosque permanente.
    public int SeedsCollected { get; private set; }

    public event Action XPChanged;
    public event Action<int> LeveledUp;

    private void Awake()
    {
        XPToNextLevel = firstLevelXP;
    }

    public void AddXP(int amount)
    {
        CurrentXP += amount;
        SeedsCollected += amount;

        // "while" y no "if": con muchas semillas de golpe se pueden subir varios niveles.
        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP -= XPToNextLevel;
            Level++;
            XPToNextLevel = Mathf.CeilToInt(XPToNextLevel * xpGrowth);

            Debug.Log($"¡Nivel {Level}!");
            LeveledUp?.Invoke(Level);
        }

        XPChanged?.Invoke();
    }
}
