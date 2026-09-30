using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Exporta el juego para Windows (.exe) con un solo clic: menú Raiz → Exportar para Windows.
// También se puede lanzar sin abrir Unity, desde la línea de comandos:
//   Unity.exe -batchmode -quit -projectPath <carpeta> -executeMethod BuildGame.BuildWindows
// Está en una carpeta "Editor": solo existe dentro de Unity, no en el juego exportado.
public static class BuildGame
{
    private const string OutputPath = "Builds/Windows/RAIZ.exe";

    [MenuItem("Raiz/Exportar para Windows")]
    public static void BuildWindows()
    {
        // Las escenas marcadas en File → Build Profiles, en su orden (Bosque primero).
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Juego exportado en {OutputPath} ({report.summary.totalSize / (1024 * 1024)} MB)");
        }
        else
        {
            Debug.LogError($"La exportación ha fallado: {report.summary.result} ({report.summary.totalErrors} errores)");

            // En modo línea de comandos, salimos con código de error para que se note el fallo.
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }
}
