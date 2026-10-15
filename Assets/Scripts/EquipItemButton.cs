using UnityEngine;
using UnityEngine.UI;

// Кнопка в лаборатории: по клику надевает предмет на персонажа.
[RequireComponent(typeof(Button))]
public class EquipItemButton : MonoBehaviour
{
    public EquipmentItem item;
    public EquipmentSystem equipmentSystem;
    [Tooltip("Необязательно: сюда подставится иконка предмета")]
    public Image icon;

    void Awake()
    {
        if (icon != null && item != null) icon.sprite = item.iconUI;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (equipmentSystem != null) equipmentSystem.Equip(item);
    }
}
