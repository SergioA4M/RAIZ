using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Interfaz en pantalla: barra de semillas (XP), nivel y vida.
public class HUD : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private Health playerHealth;

    [Header("Elementos de la interfaz")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text healthText;

    // Nos suscribimos al activarse y nos damos de baja al desactivarse,
    // para no dejar "escuchas" colgando cuando se recarga la escena.
    private void OnEnable()
    {
        experience.XPChanged += Refresh;
        experience.LeveledUp += OnLeveledUp;
        playerHealth.HealthChanged += Refresh;
    }

    private void OnDisable()
    {
        experience.XPChanged -= Refresh;
        experience.LeveledUp -= OnLeveledUp;
        playerHealth.HealthChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        // (float) para que la división dé decimales: 3 / 5 = 0.6 y no 0.
        xpBar.value = (float)experience.CurrentXP / experience.XPToNextLevel;
        levelText.text = $"Nivel {experience.Level}";
        healthText.text = $"Vida {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
    }

    private void OnLeveledUp(int level)
    {
        Refresh();
        StartCoroutine(Punch(levelText.transform));
    }

    // Agranda el texto un instante y lo devuelve a su tamaño: pequeño "¡pum!" visual.
    private IEnumerator Punch(Transform target)
    {
        const float duration = 0.25f;
        // unscaledDeltaTime: sigue funcionando aunque el juego esté en pausa
        // (al subir de nivel se abre el menú de mejoras y el tiempo se para).
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float scale = 1f + 0.5f * Mathf.Sin(t / duration * Mathf.PI);
            target.localScale = Vector3.one * scale;
            yield return null; // espera al siguiente frame
        }
        target.localScale = Vector3.one;
    }
}
