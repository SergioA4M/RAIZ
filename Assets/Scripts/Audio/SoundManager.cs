using System.Collections.Generic;
using UnityEngine;

// Reproduce efectos de sonido desde cualquier script con: SoundManager.Play(clip);
// - Tiene varias "voces" (AudioSource) para que suenen varios sonidos a la vez.
// - Cambia un poco el tono cada vez, para que un sonido repetido no canse.
// - Si el mismo sonido se pide muchas veces seguidas (20 hojas golpeando a la vez),
//   solo lo reproduce una vez cada pocos milisegundos, para no saturar.
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private int voices = 12;
    [SerializeField, Range(0f, 1f)] private float masterVolume = 0.8f;

    [Tooltip("Variación de tono al azar (0.08 = ±8 %)")]
    [SerializeField] private float pitchVariation = 0.08f;

    [Tooltip("Tiempo mínimo entre dos reproducciones del mismo sonido")]
    [SerializeField] private float minRepeatDelay = 0.05f;

    private AudioSource[] sources;
    private int nextSource;

    // Dictionary = tabla clave → valor (como HashMap en Java): sonido → última vez que sonó.
    private readonly Dictionary<AudioClip, float> lastPlayed = new Dictionary<AudioClip, float>();

    private void Awake()
    {
        Instance = this;

        // Creamos las voces por código en vez de añadirlas a mano en el Inspector.
        sources = new AudioSource[voices];
        for (int i = 0; i < voices; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
        }
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        // Si no hay SoundManager en la escena o el campo del sonido está vacío, no pasa nada.
        if (Instance == null || clip == null) return;
        Instance.PlayClip(clip, volume);
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        // unscaledTime: funciona también con el juego en pausa (menú de mejoras).
        if (lastPlayed.TryGetValue(clip, out float lastTime) && Time.unscaledTime - lastTime < minRepeatDelay)
        {
            return;
        }
        lastPlayed[clip] = Time.unscaledTime;

        // Usamos las voces por turnos (0, 1, 2… 11, 0, 1…).
        AudioSource source = sources[nextSource];
        nextSource = (nextSource + 1) % sources.Length;

        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.PlayOneShot(clip, volume * masterVolume);
    }
}
