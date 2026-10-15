using UnityEngine;

public class UICharacterEquipment : MonoBehaviour
{
    public EquipmentSystem equipmentSystem;

    public UIEquipmentSlot headSlot;
    public UIEquipmentSlot torsoSlot;
    public UIEquipmentSlot legsSlot;
    public UIEquipmentSlot feetSlot; // можно оставить пустым, если слота обуви в UI нет

    void OnEnable()
    {
        equipmentSystem.OnEquipmentChanged += UpdateUI;
        UpdateUI();
    }

    void OnDisable()
    {
        equipmentSystem.OnEquipmentChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        headSlot.Setup(equipmentSystem.equippedHead);
        torsoSlot.Setup(equipmentSystem.equippedTorso);
        legsSlot.Setup(equipmentSystem.equippedLegs);
        if (feetSlot) feetSlot.Setup(equipmentSystem.equippedFeet);
    }
}