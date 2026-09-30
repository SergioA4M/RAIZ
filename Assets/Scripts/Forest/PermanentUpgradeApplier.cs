using UnityEngine;

// Al empezar la partida, aplica al espíritu las mejoras permanentes compradas en el bosque.
public class PermanentUpgradeApplier : MonoBehaviour
{
    [SerializeField] private PermanentUpgradeCatalog catalog;

    // Start (y no Awake): así los demás componentes del espíritu ya se han preparado en su Awake.
    private void Start()
    {
        SaveData data = SaveSystem.Data;

        foreach (PermanentUpgrade upgrade in catalog.upgrades)
        {
            int level = data.GetLevel(upgrade.id);

            // Aplicar el efecto una vez por nivel: nivel 3 de "+1 vida" = +3 vidas.
            for (int i = 0; i < level; i++)
            {
                upgrade.effect.Apply(gameObject);
            }
        }
    }
}
