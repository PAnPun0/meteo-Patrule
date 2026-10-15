using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Панель шкафа с готовой одеждой. Устроена как DynamicCraftingUI и может
// использовать тот же префаб плашки (ItemIcon, ItemName, CraftBtn).
public class WardrobeUI : MonoBehaviour
{
    [Header("Данные")]
    public List<EquipmentItem> clothes = new List<EquipmentItem>();
    public EquipmentSystem equipmentSystem;

    [Header("Префаб и контейнер")]
    public Transform itemContainer;   // Объект с LayoutGroup
    public GameObject itemUIPrefab;   // Префаб плашки
    public string buttonName = "CraftBtn"; // Имя кнопки внутри префаба

    [Header("Цвет плашки надетой вещи")]
    public Color equippedColor = new Color(0.6f, 1f, 0.6f);

    [Header("Описание вещи при наведении (необязательно)")]
    public ItemInfoPanel infoPanel;

    void OnEnable()
    {
        equipmentSystem.OnEquipmentChanged += RefreshList;
        RefreshList();
    }

    void OnDisable()
    {
        equipmentSystem.OnEquipmentChanged -= RefreshList;
    }

    public void RefreshList()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (EquipmentItem item in clothes)
        {
            if (item == null) continue;

            GameObject entry = Instantiate(itemUIPrefab, itemContainer);
            entry.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.iconUI;
            entry.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;

            // Короткие характеристики, если в префабе есть текст ItemStats
            var statsText = entry.transform.Find("ItemStats");
            if (statsText) statsText.GetComponent<TextMeshProUGUI>().text = item.GetShortStatsText();
            ItemHoverInfo.Attach(entry, item, infoPanel);

            bool equipped = IsEquipped(item);
            Image background = entry.GetComponent<Image>();
            if (background && equipped) background.color = equippedColor;

            Button btn = entry.transform.Find(buttonName).GetComponent<Button>();
            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText) btnText.text = equipped ? "Снять" : "Надеть";

            // Клик по надетой вещи снимает её, по остальным — надевает
            btn.onClick.AddListener(() =>
            {
                if (IsEquipped(item)) equipmentSystem.Unequip(item.slotType);
                else equipmentSystem.Equip(item);
            });
        }
    }

    bool IsEquipped(EquipmentItem item)
    {
        return equipmentSystem.IsEquipped(item);
    }
}
