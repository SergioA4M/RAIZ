using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Herramienta de capturas automáticas para el README.
// Solo se activa si el juego se abre con la opción "-capturas <carpeta>":
//   RAIZ.exe -capturas "C:\ruta\Screenshots" -screen-fullscreen 0 -screen-width 1600 -screen-height 900
// Entonces el juego "se juega solo": visita el bosque, avanza una partida, abre el menú
// de mejoras, hace aparecer al jefe, guarda una captura de cada momento y se cierra.
// En un arranque normal no hace absolutamente nada.
public class ScreenshotDirector : MonoBehaviour
{
    private const string Flag = "-capturas";

    private string outputFolder;

    // RuntimeInitializeOnLoadMethod: Unity llama a esta función sola al arrancar el juego,
    // sin que el script tenga que estar en ninguna escena.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartIfRequested()
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, Flag);
        if (index < 0) return; // arranque normal: no hacemos nada

        GameObject host = new GameObject("ScreenshotDirector");
        DontDestroyOnLoad(host); // sobrevive a los cambios de escena

        ScreenshotDirector director = host.AddComponent<ScreenshotDirector>();
        director.outputFolder = index + 1 < args.Length ? args[index + 1] : Application.persistentDataPath;
        Directory.CreateDirectory(director.outputFolder);
        director.StartCoroutine(director.Run());
    }

    private IEnumerator Run()
    {
        // 1. El bosque, con datos de ejemplo SOLO en memoria (nunca se llama a SaveSystem.Save()).
        SaveData data = SaveSystem.Data;
        data.seeds = 137;
        data.totalRuns = 9;
        data.bestTime = 263f;
        data.bestLevel = 11;
        data.SetLevel("raices", 3);
        data.SetLevel("savia", 2);
        data.SetLevel("brisa", 3);
        data.SetLevel("brotes", 4);

        yield return LoadScene("Bosque");
        yield return new WaitForSecondsRealtime(1.5f);
        yield return Capture("bosque.png");

        // 2. Una partida avanzada: el espíritu es invencible y dispara solo.
        yield return LoadScene("SampleScene");
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        player.GetComponent<Health>().Invincible = true;
        AutoShooter shooter = player.GetComponent<AutoShooter>();
        shooter.AutoFire = true;
        shooter.AutoAim = true;

        // Avanzamos el tiempo x4 hasta el segundo 75 (ya hay orugas y muchas plagas).
        // Mientras, el espíritu da vueltas en círculo para recoger semillas, como haría un jugador:
        // desactivamos PlayerMovement (que pondría la velocidad a 0 sin teclas) y lo movemos nosotros.
        PlayerMovement movement = player;
        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        movement.enabled = false;
        float angle = 0f;
        while (Time.timeSinceLevelLoad < 75f)
        {
            PickFirstUpgradeIfMenuOpen();
            if (Time.timeScale > 0f) Time.timeScale = 4f;

            angle += Time.deltaTime; // 1 radián por segundo de juego
            body.linearVelocity = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * 4f;

            // Los últimos segundos deja de disparar para que se acumulen plagas a su alrededor.
            shooter.AutoFire = Time.timeSinceLevelLoad < 70f;
            yield return null;
        }
        body.linearVelocity = Vector2.zero;
        movement.enabled = true;
        shooter.AutoFire = true;
        Time.timeScale = 1f;
        PickFirstUpgradeIfMenuOpen();
        yield return new WaitForSecondsRealtime(0.6f);
        PickFirstUpgradeIfMenuOpen();
        yield return new WaitForSecondsRealtime(0.3f);
        yield return Capture("partida.png");

        // 3. El menú de mejoras: le damos justo la experiencia que falta para subir de nivel.
        PickFirstUpgradeIfMenuOpen();
        PlayerExperience experience = player.GetComponent<PlayerExperience>();
        experience.AddXP(experience.XPToNextLevel - experience.CurrentXP);
        yield return new WaitForSecondsRealtime(0.8f);
        yield return Capture("mejoras.png");
        PickFirstUpgradeIfMenuOpen();

        // 4. El jefe: aparece ya, lo hacemos invencible y esperamos a que se acerque.
        FindAnyObjectByType<EnemySpawner>().SpawnBossNow();
        yield return new WaitForSecondsRealtime(3f); // deja pasar el anuncio

        float timeLeft = 12f;
        while (timeLeft > 0f)
        {
            PickFirstUpgradeIfMenuOpen();
            BossCharge boss = FindAnyObjectByType<BossCharge>();
            if (boss != null)
            {
                boss.GetComponent<Health>().Invincible = true;
                if (Vector2.Distance(boss.transform.position, player.transform.position) < 4.5f) break;
            }
            timeLeft -= Time.unscaledDeltaTime;
            yield return null;
        }
        yield return Capture("jefe.png");

        Debug.Log("Capturas guardadas en " + outputFolder);
        Application.Quit();
    }

    private static IEnumerator LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        // La escena se carga en el siguiente frame; esperamos dos para ir sobre seguro.
        yield return null;
        yield return null;
    }

    private IEnumerator Capture(string fileName)
    {
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null && Camera.main != null)
        {
            Debug.Log($"[Capturas] {fileName}: jugador {player.transform.position} cámara {Camera.main.transform.position} " +
                      $"sprite visible {player.GetComponent<SpriteRenderer>().enabled} timeScale {Time.timeScale}");
        }

        ScreenCapture.CaptureScreenshot(Path.Combine(outputFolder, fileName));
        // La captura se escribe al final del frame; esperamos un poco antes de seguir.
        yield return new WaitForSecondsRealtime(0.5f);
    }

    // Si está abierto el menú de subir de nivel, pulsa la primera opción (como haría el jugador).
    private static void PickFirstUpgradeIfMenuOpen()
    {
        // GameObject.Find solo encuentra objetos activos: si el menú está oculto, devuelve null.
        GameObject menu = GameObject.Find("MenuMejoras");
        if (menu == null) return;

        Button[] buttons = menu.GetComponentsInChildren<Button>();
        if (buttons.Length > 0)
        {
            buttons[0].onClick.Invoke();
        }
    }
}
