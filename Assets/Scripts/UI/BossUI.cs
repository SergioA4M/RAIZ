using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Interfaz del jefe: anuncio en grande cuando aparece, barra de vida
// mientras está vivo y mensaje de victoria al derrotarlo.
public class BossUI : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    [Header("Elementos de la interfaz")]
    [Tooltip("Objeto que contiene la barra y el nombre del jefe (se oculta sin jefe)")]
    [SerializeField] private GameObject bossPanel;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private TMP_Text announcementText;

    [SerializeField] private float announcementDuration = 2.5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip bossArrivalSound;
    [SerializeField] private AudioClip bossDefeatedSound;

    private Health boss;

    private void Awake()
    {
        bossPanel.SetActive(false);
        announcementText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        spawner.BossSpawned += OnBossSpawned;
    }

    private void OnDisable()
    {
        spawner.BossSpawned -= OnBossSpawned;
    }

    private void OnBossSpawned(Health bossHealth)
    {
        boss = bossHealth;
        boss.HealthChanged += RefreshBar;
        boss.Died += OnBossDied;

        bossPanel.SetActive(true);
        RefreshBar();
        StartCoroutine(Announce("¡Llega la Gran Plaga!"));
        SoundManager.Play(bossArrivalSound);
    }

    private void RefreshBar()
    {
        bossHealthBar.value = (float)boss.CurrentHealth / boss.MaxHealth;
    }

    private void OnBossDied()
    {
        bossPanel.SetActive(false);
        StartCoroutine(Announce("¡Jefe derrotado!"));
        SoundManager.Play(bossDefeatedSound);
    }

    // Muestra un texto grande unos segundos y lo vuelve a ocultar.
    private IEnumerator Announce(string message)
    {
        announcementText.text = message;
        announcementText.gameObject.SetActive(true);
        yield return new WaitForSeconds(announcementDuration);
        announcementText.gameObject.SetActive(false);
    }
}
