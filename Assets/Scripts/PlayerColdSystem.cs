using UnityEngine;
using UnityEngine.UI;

public class PlayerColdSystem : MonoBehaviour
{
    public Slider coldSlider;
    public float maxWarmth = 100f;
    public float currentWarmth;
    public float freezeSpeed = 5f;
    public float insulation = 0f;
    public bool isSafeZone = false;
    private PlayerController2D controller;
    private float defaultSpeed;

    void Start()
    {
        controller = GetComponent<PlayerController2D>();
        if (controller != null)
        {
            defaultSpeed = controller.moveSpeed;
        }

        currentWarmth = maxWarmth;
        UpdateUI();
    }

    void Update()
    {
        if (isSafeZone)
        {
            currentWarmth += freezeSpeed * 2f * Time.deltaTime;
        }
        else
        {
            float actualFreeze = freezeSpeed * (1f - insulation);
            currentWarmth -= actualFreeze * Time.deltaTime;
        }

        // Ограничиваем шкалу от 0 до 100
        currentWarmth = Mathf.Clamp(currentWarmth, 0f, maxWarmth);
        UpdateUI();

        // Проверяем последствия
        CheckFreezeEffects();
    }

    void UpdateUI()
    {
        if (coldSlider != null)
        {
            coldSlider.value = currentWarmth;
        }
    }

    void CheckFreezeEffects()
    {
        if (currentWarmth <= 0f)
        {
            if (controller != null) controller.moveSpeed = defaultSpeed * 0.4f;
        }
        else
        {
            if (controller != null) controller.moveSpeed = defaultSpeed;
        }
    }

    public void EquipClothing(float clothingInsulation)
    {
        insulation = clothingInsulation;
        Debug.Log("Надета новая одежда! Защита от холода: " + (insulation * 100) + "%");
    }
}