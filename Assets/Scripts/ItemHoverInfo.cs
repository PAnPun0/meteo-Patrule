using UnityEngine;
using UnityEngine.EventSystems;

// Вешается на плашку предмета (WardrobeUI и DynamicCraftingUI добавляют его сами).
// При наведении мыши показывает вещь в ItemInfoPanel.
public class ItemHoverInfo : MonoBehaviour, IPointerEnterHandler
{
    public EquipmentItem item;
    public ItemInfoPanel infoPanel;
    public string extra; // дополнительная строка, например рецепт

    public static void Attach(GameObject entry, EquipmentItem item, ItemInfoPanel panel, string extra = null)
    {
        if (panel == null || item == null) return;
        var hover = entry.AddComponent<ItemHoverInfo>();
        hover.item = item;
        hover.infoPanel = panel;
        hover.extra = extra;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (infoPanel) infoPanel.Show(item, extra);
    }
}
