using System;
using System.IO;
using UnityEngine;

// Guarda y carga la partida en un archivo JSON en el ordenador del jugador.
// "static class": no se pone en ningún objeto; se usa directamente: SaveSystem.Data.seeds
public static class SaveSystem
{
    private const string FileName = "raiz_guardado.json";

    // persistentDataPath: carpeta que Unity reserva para cada juego y que no se borra al cerrarlo.
    // En Windows: C:\Users\<usuario>\AppData\LocalLow\<Compañía>\<Juego>\
    public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    // Se carga del disco la primera vez que se pide y luego se reutiliza (caché).
    private static SaveData cachedData;

    public static SaveData Data
    {
        get
        {
            if (cachedData == null)
            {
                cachedData = Load();
            }
            return cachedData;
        }
    }

    public static void Save()
    {
        // true = JSON con saltos de línea, más fácil de leer si abres el archivo.
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(FilePath, json);
        Debug.Log("Partida guardada en: " + FilePath);
    }

    // Útil para probar: empieza de cero.
    public static void DeleteSave()
    {
        cachedData = new SaveData();
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }

    private static SaveData Load()
    {
        if (!File.Exists(FilePath))
        {
            return new SaveData(); // primera vez que se juega
        }

        // try/catch: si el archivo está roto (por ejemplo, editado a mano),
        // en vez de cerrar el juego con un error, empezamos con datos nuevos.
        try
        {
            string json = File.ReadAllText(FilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data ?? new SaveData();
        }
        catch (Exception exception)
        {
            Debug.LogWarning("No se pudo leer el guardado, se empieza de cero: " + exception.Message);
            return new SaveData();
        }
    }
}
