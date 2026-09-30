using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menú principal del bosque: muestra las semillas y los récords,
// vende las mejoras permanentes y lleva a la partida.
public class ForestMenu : MonoBehaviour
{
    [SerializeField] private PermanentUpgradeCatalog catalog;
    [SerializeField] private ForestGrower forest;

    [Header("Interfaz")]
    [SerializeField] private TMP_Text seedsText;
    [SerializeField] private TMP_Text recordsText;

    [Tooltip("Un botón por mejora, en el mismo orden que en el catálogo")]
    [SerializeField] private Button[] upgradeButtons;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Ajustes")]
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private AudioClip buySound;

    private void Awake()
    {
        // Por si venimos de una partida en pausa.
        Time.timeScale = 1f;

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int index = i; // copia para la lambda (ver "trampa de las lambdas en bucles")
            upgradeButtons[i].onClick.AddListener(() => Buy(index));
        }

        playButton.onClick.AddListener(Play);
        quitButton.onClick.AddListener(Quit);
    }

    private void Start()
    {
        Refresh();

        // Teclado: empieza seleccionado "Jugar"; flechas para moverse por los botones
        // (los grises se saltan solos) y Enter para pulsar.
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    private void Refresh()
    {
        SaveData data = SaveSystem.Data;

        seedsText.text = $"Semillas: {data.seeds}";
        recordsText.text = data.totalRuns == 0
            ? "Aún no has defendido el bosque"
            : $"Mejor tiempo {FormatTime(data.bestTime)}  ·  Nivel máximo {data.bestLevel}  ·  Partidas {data.totalRuns}";

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            // Si hay más botones que mejoras, sobran: los ocultamos.
            bool hasUpgrade = i < catalog.upgrades.Count;
            upgradeButtons[i].gameObject.SetActive(hasUpgrade);
            if (!hasUpgrade) continue;

            PermanentUpgrade upgrade = catalog.upgrades[i];
            int level = data.GetLevel(upgrade.id);
            bool maxed = level >= upgrade.maxLevel;
            int cost = upgrade.GetCost(level);

            string costText = maxed ? "<color=#8BC34A>MÁXIMO</color>" : $"Coste: {cost}";
            upgradeButtons[i].GetComponentInChildren<TMP_Text>().text =
                $"<b>{upgrade.title}</b>  (nivel {level}/{upgrade.maxLevel})\n" +
                $"<size=70%>{upgrade.description}  ·  {costText}</size>";

            // interactable = false pone el botón gris y no deja pulsarlo.
            upgradeButtons[i].interactable = !maxed && data.seeds >= cost;
        }
    }

    private void Buy(int index)
    {
        SaveData data = SaveSystem.Data;
        PermanentUpgrade upgrade = catalog.upgrades[index];
        int level = data.GetLevel(upgrade.id);
        int cost = upgrade.GetCost(level);

        // Comprobamos otra vez: nunca hay que fiarse solo de que el botón esté activo.
        if (level >= upgrade.maxLevel || data.seeds < cost) return;

        data.seeds -= cost;
        data.SetLevel(upgrade.id, level + 1);
        SaveSystem.Save();

        SoundManager.Play(buySound);
        forest.GrowNewTree();
        Refresh();

        // Si el botón se ha quedado gris (ya no alcanza o está al máximo),
        // pasamos la selección a "Jugar" para que el teclado no se quede "atascado".
        if (!upgradeButtons[index].interactable)
        {
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }
    }

    private void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void Quit()
    {
        // En el editor de Unity no hace nada; funciona en el juego exportado (.exe).
        Application.Quit();
        Debug.Log("Salir del juego (solo funciona en la versión exportada)");
    }

    private static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return $"{total / 60:00}:{total % 60:00}";
    }
}
