using UnityEngine;

// Feet добавлен в конец, чтобы не сбить слоты у уже созданных предметов
public enum EquipSlot { Head, Torso, Legs, Feet }

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Patrol/Equipment Item")]
public class EquipmentItem : ScriptableObject
{
    [Header("Базовые параметры")]
    public string itemName;
    public EquipSlot slotType;
    public Sprite iconUI;           // Иконка для интерфейса
    public Sprite equipmentSprite;  // Спрайт, который надевается на персонажа

    [Header("Характеристики ткани")]
    [Range(0f, 1f)] public float thermalInsulation; // Тепло (0..1)
    [Range(0f, 1f)] public float windProtection;    // Защита от ветра
    [Range(0f, 1f)] public float breathability;      // Паропроницаемость

    [Header("Описание для лаборатории")]
    [TextArea(2, 6)] public string description;

    public string SlotName => OutfitRating.SlotName(slotType);

    // Строки характеристик для UI: "Тепло: 60%" и т.д.
    public string GetStatsText()
    {
        return $"Тепло: {Percent(thermalInsulation)}\n" +
               $"Защита от ветра: {Percent(windProtection)}\n" +
               $"Паропроницаемость: {Percent(breathability)}";
    }

    // Короткая строка для плашки в списке
    public string GetShortStatsText()
    {
        return $"Тепло {Percent(thermalInsulation)} · Ветер {Percent(windProtection)} · Дыхание {Percent(breathability)}";
    }

    static string Percent(float value) => Mathf.RoundToInt(value * 100f) + "%";
}