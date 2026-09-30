using System;
using System.Collections.Generic;

// Todo lo que se guarda entre partidas. Es una clase normal de C#
// (no un MonoBehaviour): solo guarda datos.
// [Serializable] permite convertirla a texto JSON con JsonUtility.
[Serializable]
public class SaveData
{
    public int seeds;       // semillas en el banco, para gastar en el bosque
    public int totalRuns;   // partidas jugadas
    public float bestTime;  // récord de tiempo aguantado (segundos)
    public int bestLevel;   // récord de nivel alcanzado

    // Nivel comprado de cada mejora permanente.
    // JsonUtility no sabe guardar Dictionary, así que usamos una lista de parejas (id, nivel).
    public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();

    public int GetLevel(string upgradeId)
    {
        foreach (UpgradeLevel entry in upgradeLevels)
        {
            if (entry.id == upgradeId) return entry.level;
        }
        return 0;
    }

    public void SetLevel(string upgradeId, int level)
    {
        foreach (UpgradeLevel entry in upgradeLevels)
        {
            if (entry.id == upgradeId)
            {
                entry.level = level;
                return;
            }
        }
        upgradeLevels.Add(new UpgradeLevel { id = upgradeId, level = level });
    }

    // Suma de todos los niveles comprados: lo usará el bosque para saber cuántos árboles dibujar.
    public int TotalUpgradeLevels()
    {
        int total = 0;
        foreach (UpgradeLevel entry in upgradeLevels)
        {
            total += entry.level;
        }
        return total;
    }
}

[Serializable]
public class UpgradeLevel
{
    public string id;
    public int level;
}
