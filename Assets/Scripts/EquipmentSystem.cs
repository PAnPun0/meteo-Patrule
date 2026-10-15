using UnityEngine;
using System;

public class EquipmentSystem : MonoBehaviour
{
    [Header("Надетые предметы")]
    public EquipmentItem equippedHead;
    public EquipmentItem equippedTorso;
    public EquipmentItem equippedLegs;
    public EquipmentItem equippedFeet;

    [Header("Ссылки на рендереры персонажа")]
    public SpriteRenderer headRenderer;
    public SpriteRenderer torsoRenderer;
    public SpriteRenderer legsRenderer;
    public SpriteRenderer feetRenderer;

    [Header("Связи")]
    public PlayerColdSystem coldSystem;

    // Событие для обновления UI
    public event Action OnEquipmentChanged;

    void Start()
    {
        // Восстанавливаем одежду, выбранную в другой сцене (например, в лаборатории)
        // Копируем заранее: Equip() перезаписывает GameSession через RecalculateStats
        var head = GameSession.head;
        var torso = GameSession.torso;
        var legs = GameSession.legs;
        var feet = GameSession.feet;
        if (head) Equip(head);
        if (torso) Equip(torso);
        if (legs) Equip(legs);
        if (feet) Equip(feet);
    }

    public void Equip(EquipmentItem item)
    {
        if (item == null) return;

        switch (item.slotType)
        {
            case EquipSlot.Head:
                equippedHead = item;
                if (headRenderer) headRenderer.sprite = item.equipmentSprite;
                break;
            case EquipSlot.Torso:
                equippedTorso = item;
                if (torsoRenderer) torsoRenderer.sprite = item.equipmentSprite;
                break;
            case EquipSlot.Legs:
                equippedLegs = item;
                if (legsRenderer) legsRenderer.sprite = item.equipmentSprite;
                break;
            case EquipSlot.Feet:
                equippedFeet = item;
                if (feetRenderer) feetRenderer.sprite = item.equipmentSprite;
                break;
        }

        RecalculateStats();
        OnEquipmentChanged?.Invoke();
    }

    public void Unequip(EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.Head:
                equippedHead = null;
                if (headRenderer) headRenderer.sprite = null;
                break;
            case EquipSlot.Torso:
                equippedTorso = null;
                if (torsoRenderer) torsoRenderer.sprite = null;
                break;
            case EquipSlot.Legs:
                equippedLegs = null;
                if (legsRenderer) legsRenderer.sprite = null;
                break;
            case EquipSlot.Feet:
                equippedFeet = null;
                if (feetRenderer) feetRenderer.sprite = null;
                break;
        }

        RecalculateStats();
        OnEquipmentChanged?.Invoke();
    }

    public EquipmentItem GetEquipped(EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.Head: return equippedHead;
            case EquipSlot.Torso: return equippedTorso;
            case EquipSlot.Legs: return equippedLegs;
            case EquipSlot.Feet: return equippedFeet;
        }
        return null;
    }

    public bool IsEquipped(EquipmentItem item)
    {
        return item != null && GetEquipped(item.slotType) == item;
    }

    private void RecalculateStats()
    {
        float totalInsulation = 0f;

        if (equippedHead) totalInsulation += equippedHead.thermalInsulation;
        if (equippedTorso) totalInsulation += equippedTorso.thermalInsulation;
        if (equippedLegs) totalInsulation += equippedLegs.thermalInsulation;
        if (equippedFeet) totalInsulation += equippedFeet.thermalInsulation;

        // Нормализуем/передаем в систему замерзания (максимум 95% защиты)
        totalInsulation = Mathf.Clamp(totalInsulation, 0f, 0.95f);
        if (coldSystem != null)
        {
            coldSystem.EquipClothing(totalInsulation);
        }

        // Запоминаем одежду для следующей сцены
        GameSession.head = equippedHead;
        GameSession.torso = equippedTorso;
        GameSession.legs = equippedLegs;
        GameSession.feet = equippedFeet;
    }
}
