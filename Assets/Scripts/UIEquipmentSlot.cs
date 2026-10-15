using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour
{
    public EquipSlot slotType;
    public Image itemIcon;
    public Button unequipButton;
    public EquipmentSystem equipmentSystem;

    public void Setup(EquipmentItem item)
    {
        if (item != null)
        {
            itemIcon.sprite = item.iconUI;
            itemIcon.enabled = true;
            if (unequipButton) unequipButton.interactable = true;
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            if (unequipButton) unequipButton.interactable = false;
        }
    }

    // При клике на иконку — снимаем предмет
    public void OnClickSlot()
    {
        equipmentSystem.Unequip(slotType);
    }
}