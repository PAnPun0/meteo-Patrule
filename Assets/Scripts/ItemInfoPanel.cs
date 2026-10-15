using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Окно с описанием вещи в лаборатории: название, слот, описание, характеристики.
// Заполняется при наведении мыши на плашку в шкафу или на верстаке (см. ItemHoverInfo).
public class ItemInfoPanel : MonoBehaviour
{
    [Header("Тексты")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI slotText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statsText;   // "Тепло: 60%" ...
    public TextMeshProUGUI extraText;   // для верстака: что нужно для крафта (необязательно)

    [Header("Иконка (необязательно)")]
    public Image icon;

    [Header("Полоски характеристик (необязательно): заливка-RectTransform внутри фона")]
    public RectTransform insulationFill;
    public RectTransform windFill;
    public RectTransform breathFill;

    [TextArea] public string emptyHint = "Наведи курсор на вещь, чтобы увидеть её описание";

    void OnEnable()
    {
        Clear();
    }

    public void Show(EquipmentItem item, string extra = null)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (nameText) nameText.text = item.itemName;
        if (slotText) slotText.text = "Слот: " + item.SlotName;
        if (descriptionText)
            descriptionText.text = string.IsNullOrEmpty(item.description) ? "Описание не задано" : item.description;
        if (statsText) statsText.text = item.GetStatsText();
        if (extraText) extraText.text = extra ?? "";

        if (icon)
        {
            icon.sprite = item.iconUI;
            icon.enabled = item.iconUI != null;
        }

        SetFill(insulationFill, item.thermalInsulation);
        SetFill(windFill, item.windProtection);
        SetFill(breathFill, item.breathability);
    }

    public void Clear()
    {
        if (nameText) nameText.text = "";
        if (slotText) slotText.text = "";
        if (descriptionText) descriptionText.text = emptyHint;
        if (statsText) statsText.text = "";
        if (extraText) extraText.text = "";
        if (icon) icon.enabled = false;

        SetFill(insulationFill, 0f);
        SetFill(windFill, 0f);
        SetFill(breathFill, 0f);
    }

    static void SetFill(RectTransform fill, float value)
    {
        if (fill == null) return;
        // Заливка растянута по фону: меняем правый край по ширине
        fill.anchorMin = new Vector2(0f, 0f);
        fill.anchorMax = new Vector2(Mathf.Clamp01(value), 1f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
    }
}
