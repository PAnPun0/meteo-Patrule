using System.Collections.Generic;
using UnityEngine;

// Требования уровня к одежде. Задаются в инспекторе MissionEnd.
[System.Serializable]
public class OutfitRequirements
{
    [Tooltip("Сколько тепла нужно в сумме по всем вещам (сумма thermalInsulation)")]
    public float targetInsulation = 1.2f;
    [Tooltip("Насколько можно превысить тепло, прежде чем оценка за тепло станет 0 (перегрев)")]
    public float overheatTolerance = 0.8f;
    [Range(0f, 1f), Tooltip("Средняя защита от ветра по 4 слотам")]
    public float targetWindProtection = 0.5f;
    [Range(0f, 1f), Tooltip("Средняя паропроницаемость надетых вещей")]
    public float targetBreathability = 0.4f;
}

public struct OutfitResult
{
    public int score;       // 0..100
    public int stars;       // 0..3
    public string grade;
    public List<string> feedback;
}

// Подсчёт оценки за экипировку. Веса: слоты 25, тепло 35, ветер 20, дыхание 10, состояние 10.
public static class OutfitRating
{
    static readonly EquipSlot[] AllSlots = { EquipSlot.Head, EquipSlot.Torso, EquipSlot.Legs, EquipSlot.Feet };

    public static OutfitResult Evaluate(EquipmentSystem equipment, PlayerColdSystem cold, OutfitRequirements req)
    {
        var feedback = new List<string>();

        int filled = 0;
        float insulation = 0f, wind = 0f, breath = 0f;
        foreach (var slot in AllSlots)
        {
            var item = equipment != null ? equipment.GetEquipped(slot) : null;
            if (item == null)
            {
                feedback.Add("Не надето: " + SlotName(slot));
                continue;
            }
            filled++;
            insulation += item.thermalInsulation;
            wind += item.windProtection;
            breath += item.breathability;
        }

        // 1. Все ли части тела закрыты
        float coverageScore = 25f * filled / AllSlots.Length;

        // 2. Тепло: недобор и перегрев одинаково плохо
        float warmthFactor;
        float diff = insulation - req.targetInsulation;
        if (diff < 0f)
        {
            warmthFactor = req.targetInsulation > 0f ? 1f + diff / req.targetInsulation : 1f;
            if (warmthFactor < 0.8f) feedback.Add("Слишком легко оделись — холодно");
        }
        else
        {
            warmthFactor = req.overheatTolerance > 0f ? 1f - diff / req.overheatTolerance : 1f;
            if (warmthFactor < 0.8f) feedback.Add("Слишком тепло — перегрев и пот");
        }
        float warmthScore = 35f * Mathf.Clamp01(warmthFactor);

        // 3. Ветер: среднее по всем слотам, пустой слот = 0
        float windAvg = wind / AllSlots.Length;
        float windFactor = req.targetWindProtection > 0f ? Mathf.Clamp01(windAvg / req.targetWindProtection) : 1f;
        if (windFactor < 0.8f) feedback.Add("Мало защиты от ветра");
        float windScore = 20f * windFactor;

        // 4. Паропроницаемость: среднее по надетым вещам
        float breathAvg = filled > 0 ? breath / filled : 0f;
        float breathFactor = req.targetBreathability > 0f ? Mathf.Clamp01(breathAvg / req.targetBreathability) : 1f;
        if (filled > 0 && breathFactor < 0.8f) feedback.Add("Одежда плохо «дышит»");
        float breathScore = 10f * breathFactor;

        // 5. Сколько тепла осталось у персонажа к концу миссии
        float survivalFactor = cold != null && cold.maxWarmth > 0f ? cold.currentWarmth / cold.maxWarmth : 1f;
        if (survivalFactor < 0.3f) feedback.Add("Персонаж сильно замёрз в пути");
        float survivalScore = 10f * Mathf.Clamp01(survivalFactor);

        int score = Mathf.RoundToInt(coverageScore + warmthScore + windScore + breathScore + survivalScore);

        int stars;
        string grade;
        if (score >= 85) { stars = 3; grade = "Отлично"; }
        else if (score >= 60) { stars = 2; grade = "Хорошо"; }
        else if (score >= 35) { stars = 1; grade = "Удовлетворительно"; }
        else { stars = 0; grade = "Плохо"; }

        if (feedback.Count == 0) feedback.Add("Экипировка подобрана идеально!");

        return new OutfitResult { score = score, stars = stars, grade = grade, feedback = feedback };
    }

    public static string SlotName(EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.Head: return "головной убор";
            case EquipSlot.Torso: return "куртка";
            case EquipSlot.Legs: return "штаны";
            case EquipSlot.Feet: return "обувь";
        }
        return slot.ToString();
    }
}
