using TMPro;
using UnityEngine;

// Número de daño que flota hacia arriba, hace un pequeño "¡pum!" y se desvanece.
// Es texto "en el mundo" (TextMeshPro normal, no de UI): vive en la escena
// junto a los enemigos, no dentro del Canvas.
[RequireComponent(typeof(TextMeshPro))]
public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float riseSpeed = 1.5f;

    private TextMeshPro text;
    private Color startColor;
    private Vector3 startScale;
    private float age;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        startColor = text.color;
        startScale = transform.localScale;

        // Orden de dibujo alto para que se vea por encima de enemigos y partículas.
        text.sortingOrder = 20;
    }

    // La llama DamagePopup justo después de crearlo.
    public void Setup(int amount)
    {
        text.text = amount.ToString();
    }

    private void Update()
    {
        age += Time.deltaTime;
        float progress = age / lifetime; // 0 al nacer, 1 al desaparecer

        // Sube poco a poco.
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Crece un instante al principio (el primer 25 % de su vida) y vuelve a su tamaño.
        float punch = Mathf.Sin(Mathf.Clamp01(progress * 4f) * Mathf.PI) * 0.4f;
        transform.localScale = startScale * (1f + punch);

        // Se vuelve transparente: el canal "a" (alfa) del color va de 1 a 0.
        text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - progress);

        if (age >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
