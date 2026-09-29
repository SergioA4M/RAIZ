using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Al subir de nivel: pausa el juego y ofrece 3 mejoras al azar.
// Se elige con el ratón o con las teclas 1, 2 y 3.
public class LevelUpMenu : MonoBehaviour
{
    [SerializeField] private PlayerExperience experience;

    // El panel que se muestra y se oculta (este script NO va dentro de él,
    // porque un objeto oculto no recibe eventos).
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] optionButtons;

    // Todas las mejoras posibles. De aquí se sacan 3 al azar.
    [SerializeField] private List<UpgradeData> upgrades;

    [Header("Sonidos")]
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip chooseSound;

    private readonly List<UpgradeData> currentOptions = new List<UpgradeData>();
    private int pendingLevelUps;

    private void Awake()
    {
        // Por si la escena se recargó con el juego en pausa.
        Time.timeScale = 1f;
        panel.SetActive(false);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            // Copiamos i en una variable nueva: si usáramos i directamente,
            // todos los botones acabarían usando su último valor.
            int index = i;
            optionButtons[i].onClick.AddListener(() => Choose(index));
        }
    }

    private void OnEnable()
    {
        experience.LeveledUp += OnLeveledUp;
    }

    private void OnDisable()
    {
        experience.LeveledUp -= OnLeveledUp;
    }

    // Update sigue funcionando en pausa (solo se para lo que usa Time.deltaTime).
    private void Update()
    {
        if (!panel.activeSelf || Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) Choose(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) Choose(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) Choose(2);
    }

    private void OnLeveledUp(int level)
    {
        // Si se suben varios niveles de golpe, se elige una mejora por cada uno.
        pendingLevelUps++;
        if (!panel.activeSelf)
        {
            Show();
        }
    }

    private void Show()
    {
        // Sacamos mejoras al azar de una copia de la lista, quitando
        // cada una elegida para que no salgan repetidas.
        currentOptions.Clear();
        List<UpgradeData> pool = new List<UpgradeData>(upgrades);
        while (currentOptions.Count < optionButtons.Length && pool.Count > 0)
        {
            int randomIndex = Random.Range(0, pool.Count);
            currentOptions.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            bool hasOption = i < currentOptions.Count;
            optionButtons[i].gameObject.SetActive(hasOption);

            if (hasOption)
            {
                UpgradeData upgrade = currentOptions[i];
                // TextMeshPro entiende etiquetas: <b> negrita, <size> tamaño.
                optionButtons[i].GetComponentInChildren<TMP_Text>().text =
                    $"{i + 1}. <b>{upgrade.title}</b>\n<size=70%>{upgrade.description}</size>";
            }
        }

        panel.SetActive(true);
        SoundManager.Play(levelUpSound);

        // timeScale = 0 congela todo lo que depende del tiempo: física, disparos, enemigos...
        Time.timeScale = 0f;
    }

    private void Choose(int index)
    {
        if (index >= currentOptions.Count) return;

        currentOptions[index].Apply(experience.gameObject);
        SoundManager.Play(chooseSound);
        pendingLevelUps--;

        if (pendingLevelUps > 0)
        {
            Show();
        }
        else
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
