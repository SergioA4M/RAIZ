using UnityEngine;

public enum UpgradeType
{
    FireRate,        // dispara más rápido
    Damage,          // más daño por hoja
    ExtraProjectile, // una hoja más por disparo
    MoveSpeed,       // moverse más rápido
    MaxHealth        // más vida máxima (y cura)
}

// ScriptableObject: un archivo de datos que vive en la carpeta Assets.
// Cada mejora es un archivo distinto que se crea desde el menú
// Create → Raiz → Mejora, sin tocar código.
[CreateAssetMenu(fileName = "NuevaMejora", menuName = "Raiz/Mejora")]
public class UpgradeData : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public UpgradeType type;

    // Qué cantidad sube. En porcentajes: 0.2 = +20 %.
    public float amount;

    public void Apply(GameObject player)
    {
        switch (type)
        {
            case UpgradeType.FireRate:
                player.GetComponent<AutoShooter>().IncreaseFireRate(amount);
                break;
            case UpgradeType.Damage:
                player.GetComponent<AutoShooter>().AddDamage(Mathf.RoundToInt(amount));
                break;
            case UpgradeType.ExtraProjectile:
                player.GetComponent<AutoShooter>().AddProjectiles(Mathf.RoundToInt(amount));
                break;
            case UpgradeType.MoveSpeed:
                player.GetComponent<PlayerMovement>().IncreaseSpeed(amount);
                break;
            case UpgradeType.MaxHealth:
                player.GetComponent<Health>().IncreaseMaxHealth(Mathf.RoundToInt(amount));
                break;
        }
    }
}
