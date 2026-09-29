using System.Collections;
using UnityEngine;

// "Hit stop": congela el juego una fracción de segundo en un golpe importante.
// Casi no se ve, pero se siente: da sensación de impacto (lo usan casi todos los juegos de acción).
public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    // Usamos un tiempo casi parado (no 0) para distinguirlo de la pausa del menú de mejoras.
    private const float FrozenTimeScale = 0.02f;

    private void Awake()
    {
        Instance = this;
    }

    public static void Freeze(float duration)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.FreezeRoutine(duration));
        }
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        // Si el juego ya está en pausa (menú de mejoras), no hacemos nada.
        if (Time.timeScale == 0f) yield break;

        Time.timeScale = FrozenTimeScale;

        // Realtime: esperamos en tiempo real, porque el tiempo del juego está casi parado.
        yield return new WaitForSecondsRealtime(duration);

        // Solo lo devolvemos a 1 si nadie lo ha cambiado mientras tanto
        // (por ejemplo, si se abrió el menú de mejoras justo en ese momento).
        if (Time.timeScale == FrozenTimeScale)
        {
            Time.timeScale = 1f;
        }
    }
}
