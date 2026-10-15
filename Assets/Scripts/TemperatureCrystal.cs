using UnityEngine;
using UnityEngine.UI;

// Кристалл над персонажем, показывающий его температуру.
// Значение берётся из ColdSlider (тот же слайдер, что использует PlayerColdSystem).
[RequireComponent(typeof(SpriteRenderer))]
public class TemperatureCrystal : MonoBehaviour
{
    [Header("Источник температуры")]
    public Slider coldSlider;

    [Header("Спрайты кристалла: элемент 0 = тепло, последний = замёрз")]
    public Sprite[] stageSprites;

    [Header("Покачивание")]
    public float bobAmplitude = 0.05f;
    public float bobSpeed = 2f;

    [Header("Пульсация при сильном холоде")]
    [Range(0f, 1f)] public float pulseThreshold = 0.25f;
    public float pulseSpeed = 6f;
    public float pulseScale = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Vector3 baseLocalPosition;
    private Vector3 baseScale;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseLocalPosition = transform.localPosition;
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (coldSlider == null) return;

        // 0 = полностью замёрз, 1 = полностью тепло
        float warmth = coldSlider.normalizedValue;

        UpdateSprite(warmth);
        Animate(warmth);
    }

    void UpdateSprite(float warmth)
    {
        if (stageSprites == null || stageSprites.Length == 0) return;

        // Шкала делится поровну на количество спрайтов.
        // warmth 1 -> индекс 0 (тепло), warmth 0 -> последний индекс (замёрз)
        int index = Mathf.Clamp(
            Mathf.FloorToInt((1f - warmth) * stageSprites.Length),
            0, stageSprites.Length - 1);

        if (stageSprites[index] != null && spriteRenderer.sprite != stageSprites[index])
            spriteRenderer.sprite = stageSprites[index];
    }

    void Animate(float warmth)
    {
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.localPosition = baseLocalPosition + new Vector3(0f, bob, 0f);

        if (warmth <= pulseThreshold)
        {
            float pulse = 1f + Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed)) * pulseScale;
            transform.localScale = baseScale * pulse;
        }
        else
        {
            transform.localScale = baseScale;
        }
    }
}
