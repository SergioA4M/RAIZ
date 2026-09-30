using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Pantalla de fin de partida: guarda las semillas en el banco, muestra el resumen
// y deja elegir entre reintentar o volver al bosque.
public class GameOverScreen : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerExperience experience;

    [Header("Interfaz")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button forestButton;

    [Header("Ajustes")]
    [SerializeField] private string forestSceneName = "Bosque";
    [SerializeField] private float delayBeforeShowing = 1.5f;
    [SerializeField] private AudioClip gameOverSound;

    private void Awake()
    {
        panel.SetActive(false);
        retryButton.onClick.AddListener(Retry);
        forestButton.onClick.AddListener(GoToForest);
    }

    private void OnEnable()
    {
        playerHealth.Died += OnPlayerDied;
    }

    private void OnDisable()
    {
        playerHealth.Died -= OnPlayerDied;
    }

    // Atajos de teclado: R = reintentar, B = bosque.
    private void Update()
    {
        if (!panel.activeSelf || Keyboard.current == null) return;

        if (Keyboard.current.rKey.wasPressedThisFrame) Retry();
        else if (Keyboard.current.bKey.wasPressedThisFrame) GoToForest();
    }

    private void OnPlayerDied()
    {
        // Guardamos YA, antes de esperar: si el jugador cierra el juego
        // durante la espera, no pierde sus semillas.
        float timeSurvived = Time.timeSinceLevelLoad;
        int seedsEarned = experience.SeedsCollected;

        SaveData data = SaveSystem.Data;
        data.seeds += seedsEarned;
        data.totalRuns++;
        bool newRecord = timeSurvived > data.bestTime;
        if (newRecord) data.bestTime = timeSurvived;
        data.bestLevel = Mathf.Max(data.bestLevel, experience.Level);
        SaveSystem.Save();

        string recordText = newRecord ? "  <color=#FFD54A>¡Récord!</color>" : "";
        statsText.text =
            $"Has resistido {FormatTime(timeSurvived)}{recordText}\n" +
            $"Nivel {experience.Level}\n" +
            $"+{seedsEarned} semillas  (en tu bosque: {data.seeds})";

        StartCoroutine(ShowAfterDelay());
    }

    private IEnumerator ShowAfterDelay()
    {
        // Una pequeña pausa para ver la muerte antes del resumen.
        yield return new WaitForSeconds(delayBeforeShowing);

        panel.SetActive(true);
        SoundManager.Play(gameOverSound);

        // Teclado: flechas para elegir botón y Enter para pulsarlo (además de R y B).
        EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
        Time.timeScale = 0f; // congelamos a los enemigos detrás del panel
    }

    private void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToForest()
    {
        // Comprobamos que la escena existe y está en la lista de escenas del juego,
        // para dar un aviso claro en vez de un error.
        if (!Application.CanStreamedLevelBeLoaded(forestSceneName))
        {
            Debug.LogWarning($"La escena '{forestSceneName}' todavía no existe o no está en File → Build Profiles.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(forestSceneName);
    }

    private static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return $"{total / 60:00}:{total % 60:00}";
    }
}
