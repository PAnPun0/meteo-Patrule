using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Меню Tools → Lab → Собрать панели верстака и шкафа.
// Наполняет пустые панели, на которые ссылаются LabClickable в открытой сцене:
// заголовок, кнопка закрытия, прокручиваемый список, материалы (у верстака)
// и подключает DynamicCraftingUI / WardrobeUI со всеми ссылками.
public static class LabPanelsBuilder
{
    const string EntryPrefabPath = "Assets/RecipyEntry_Prefab.prefab";

    // Имена создаваемых объектов: при повторном запуске они пересоздаются
    static readonly string[] GeneratedNames = { "Title", "CloseButton", "Scroll", "Materials", "InfoPanel" };

    [MenuItem("Tools/Lab/Собрать панели верстака и шкафа")]
    public static void Build()
    {
        var clickables = Object.FindObjectsByType<LabClickable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (clickables.Length == 0)
        {
            EditorUtility.DisplayDialog("Lab", "В сцене нет объектов с LabClickable. Открой сцену Laboratory.", "OK");
            return;
        }

        var equipment = Object.FindFirstObjectByType<EquipmentSystem>(FindObjectsInactive.Include);
        var inventory = Object.FindFirstObjectByType<PlayerInventory>(FindObjectsInactive.Include);
        var entryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EntryPrefabPath);
        if (entryPrefab == null)
            Debug.LogWarning($"Не найден префаб {EntryPrefabPath} — укажи Prefab в панелях вручную.");

        var recipes = LoadAll<CraftRecipe>();
        var craftResults = new HashSet<EquipmentItem>(recipes.Where(r => r.resultItem).Select(r => r.resultItem));
        // В шкаф — вся одежда, которую нельзя скрафтить
        var readyClothes = LoadAll<EquipmentItem>().Where(i => !craftResults.Contains(i)).ToList();

        foreach (var clickable in clickables)
        {
            if (clickable.panel == null)
            {
                Debug.LogWarning($"У «{clickable.name}» не указана Panel — пропускаю.", clickable);
                continue;
            }

            var oldCrafting = clickable.GetComponent<DynamicCraftingUI>();
            bool isWorkbench = oldCrafting != null
                || clickable.panel.GetComponent<DynamicCraftingUI>() != null
                || clickable.name.ToLower().Contains("верст")
                || clickable.name.ToLower().Contains("work");

            if (isWorkbench)
                BuildCraftingPanel(clickable, oldCrafting, recipes, inventory, equipment, entryPrefab);
            else
                BuildWardrobePanel(clickable, readyClothes, equipment, entryPrefab);
        }

        EditorSceneManager.MarkSceneDirty(clickables[0].gameObject.scene);
        Debug.Log("Панели лаборатории собраны. Не забудь сохранить сцену (Ctrl+S).");
    }

    static void BuildCraftingPanel(LabClickable owner, DynamicCraftingUI oldCrafting, List<CraftRecipe> recipes,
        PlayerInventory inventory, EquipmentSystem equipment, GameObject entryPrefab)
    {
        var panel = owner.panel;
        ClearGenerated(panel.transform);

        CreateTitle(panel.transform, "Верстак");
        CreateCloseButton(panel.transform, owner);
        var content = CreateScrollList(panel.transform, bottomPadding: 60f);

        // Материалы внизу панели
        var materials = CreateUI("Materials", panel.transform);
        var mRect = materials.GetComponent<RectTransform>();
        mRect.anchorMin = new Vector2(0f, 0f);
        mRect.anchorMax = new Vector2(1f, 0f);
        mRect.pivot = new Vector2(0.5f, 0f);
        mRect.offsetMin = new Vector2(10f, 10f);
        mRect.offsetMax = new Vector2(-10f, 50f);
        var hLayout = materials.AddComponent<HorizontalLayoutGroup>();
        hLayout.childForceExpandWidth = true;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.spacing = 10f;

        var woolText = CreateText("WoolText", materials.transform, "Шерсть: 0", 20);
        var fleeceText = CreateText("FleeceText", materials.transform, "Флис: 0", 20);
        var membraneText = CreateText("MembraneText", materials.transform, "Мембрана: 0", 20);

        if (inventory != null)
        {
            Undo.RecordObject(inventory, "Link inventory texts");
            inventory.woolText = woolText;
            inventory.fleeceText = fleeceText;
            inventory.membraneText = membraneText;
            EditorUtility.SetDirty(inventory);
        }

        // DynamicCraftingUI должен стоять на панели: OnEnable обновляет список при каждом открытии
        var crafting = panel.GetComponent<DynamicCraftingUI>();
        if (crafting == null) crafting = Undo.AddComponent<DynamicCraftingUI>(panel);
        Undo.RecordObject(crafting, "Setup crafting UI");

        if (oldCrafting != null && oldCrafting.allRecipes.Count > 0)
            crafting.allRecipes = new List<CraftRecipe>(oldCrafting.allRecipes);
        else if (crafting.allRecipes.Count == 0)
            crafting.allRecipes = new List<CraftRecipe>(recipes);

        crafting.playerInventory = inventory;
        crafting.equipmentSystem = equipment;
        crafting.recipeContainer = content;
        if (entryPrefab) crafting.recipeUIPrefab = entryPrefab;
        crafting.infoPanel = CreateInfoPanel(panel.transform, withExtra: true);
        EditorUtility.SetDirty(crafting);

        // Старый компонент на самом верстаке больше не нужен (и падал бы без контейнера)
        if (oldCrafting != null) Undo.DestroyObjectImmediate(oldCrafting);
    }

    static void BuildWardrobePanel(LabClickable owner, List<EquipmentItem> clothes,
        EquipmentSystem equipment, GameObject entryPrefab)
    {
        var panel = owner.panel;
        ClearGenerated(panel.transform);

        CreateTitle(panel.transform, "Шкаф");
        CreateCloseButton(panel.transform, owner);
        var content = CreateScrollList(panel.transform, bottomPadding: 10f);

        var wardrobe = panel.GetComponent<WardrobeUI>();
        if (wardrobe == null) wardrobe = Undo.AddComponent<WardrobeUI>(panel);
        Undo.RecordObject(wardrobe, "Setup wardrobe UI");

        if (wardrobe.clothes.Count == 0) wardrobe.clothes = new List<EquipmentItem>(clothes);
        wardrobe.equipmentSystem = equipment;
        wardrobe.itemContainer = content;
        if (entryPrefab) wardrobe.itemUIPrefab = entryPrefab;
        wardrobe.infoPanel = CreateInfoPanel(panel.transform, withExtra: false);
        EditorUtility.SetDirty(wardrobe);
    }

    // ---------- Построение UI ----------

    static void CreateTitle(Transform parent, string text)
    {
        var title = CreateText("Title", parent, text, 32);
        title.alignment = TextAlignmentOptions.Center;
        title.fontStyle = FontStyles.Bold;
        var rect = title.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(10f, -55f);
        rect.offsetMax = new Vector2(-55f, -5f);
    }

    static void CreateCloseButton(Transform parent, LabClickable owner)
    {
        var go = CreateUI("CloseButton", parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-8f, -8f);
        rect.sizeDelta = new Vector2(40f, 40f);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.8f, 0.25f, 0.25f);
        var button = go.AddComponent<Button>();
        button.targetGraphic = image;

        var label = CreateText("Text", go.transform, "X", 24);
        label.alignment = TextAlignmentOptions.Center;
        Stretch(label.rectTransform, 0f);

        // Постоянный обработчик, виден в инспекторе кнопки
        UnityEventTools.AddVoidPersistentListener(button.onClick, new UnityAction(owner.Close));
    }

    // Возвращает Content — контейнер для плашек
    static Transform CreateScrollList(Transform parent, float bottomPadding)
    {
        var scroll = CreateUI("Scroll", parent);
        var sRect = scroll.GetComponent<RectTransform>();
        sRect.anchorMin = Vector2.zero;
        sRect.anchorMax = Vector2.one;
        sRect.offsetMin = new Vector2(10f, bottomPadding);
        sRect.offsetMax = new Vector2(-10f, -60f);
        var scrollRect = scroll.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 20f;

        var viewport = CreateUI("Viewport", scroll.transform);
        Stretch(viewport.GetComponent<RectTransform>(), 0f);
        viewport.AddComponent<RectMask2D>();
        // Прозрачный Image, чтобы список ловил колесо мыши и перетаскивание
        var vpImage = viewport.AddComponent<Image>();
        vpImage.color = new Color(1f, 1f, 1f, 0f);

        var content = CreateUI("Content", viewport.transform);
        var cRect = content.GetComponent<RectTransform>();
        cRect.anchorMin = new Vector2(0f, 1f);
        cRect.anchorMax = new Vector2(1f, 1f);
        cRect.pivot = new Vector2(0.5f, 1f);
        cRect.offsetMin = Vector2.zero;
        cRect.offsetMax = Vector2.zero;

        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.padding = new RectOffset(5, 5, 5, 5);
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandHeight = false;
        var fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = cRect;
        return content.transform;
    }

    // Окно описания вещи рядом с панелью (дочернее, поэтому закрывается вместе с ней).
    // Ставится со стороны центра экрана, чтобы не уйти за край.
    static ItemInfoPanel CreateInfoPanel(Transform panel, bool withExtra)
    {
        var panelRect = (RectTransform)panel;
        bool panelOnLeft = (panelRect.anchorMin.x + panelRect.anchorMax.x) * 0.5f < 0.5f;

        var go = CreateUI("InfoPanel", panel);
        var rect = go.GetComponent<RectTransform>();
        if (panelOnLeft)
        {
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(10f, 0f);
        }
        else
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-10f, 0f);
        }
        rect.sizeDelta = new Vector2(300f, 0f);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.9f);

        var layout = go.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 6f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var info = Undo.AddComponent<ItemInfoPanel>(go);

        info.nameText = CreateText("Name", go.transform, "", 26);
        info.nameText.fontStyle = FontStyles.Bold;
        info.slotText = CreateText("Slot", go.transform, "", 18);

        var iconGo = CreateUI("Icon", go.transform);
        info.icon = iconGo.AddComponent<Image>();
        info.icon.preserveAspect = true;
        info.icon.raycastTarget = false;
        var iconLayout = iconGo.AddComponent<LayoutElement>();
        iconLayout.preferredHeight = 80f;

        info.descriptionText = CreateText("Description", go.transform, "", 18);
        info.descriptionText.alignment = TextAlignmentOptions.TopLeft;
        info.descriptionText.textWrappingMode = TextWrappingModes.Normal;
        var descLayout = info.descriptionText.gameObject.AddComponent<LayoutElement>();
        descLayout.flexibleHeight = 1f; // описание занимает всё свободное место

        info.statsText = CreateText("Stats", go.transform, "", 18);
        info.statsText.alignment = TextAlignmentOptions.Left;

        info.insulationFill = CreateStatBar(go.transform, "Тепло", new Color(1f, 0.55f, 0.2f));
        info.windFill = CreateStatBar(go.transform, "Ветер", new Color(0.4f, 0.7f, 1f));
        info.breathFill = CreateStatBar(go.transform, "Дыхание", new Color(0.4f, 0.85f, 0.4f));

        if (withExtra)
        {
            info.extraText = CreateText("Extra", go.transform, "", 18);
            info.extraText.fontStyle = FontStyles.Italic;
        }

        EditorUtility.SetDirty(info);
        return info;
    }

    // Строка "Название [=====     ]", возвращает заливку
    static RectTransform CreateStatBar(Transform parent, string label, Color color)
    {
        var row = CreateUI("Bar_" + label, parent);
        row.AddComponent<LayoutElement>().preferredHeight = 22f;
        var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = 8f;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = false;

        var text = CreateText("Label", row.transform, label, 16);
        text.alignment = TextAlignmentOptions.Left;
        text.gameObject.AddComponent<LayoutElement>().preferredWidth = 80f;

        var back = CreateUI("Back", row.transform);
        back.AddComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
        back.AddComponent<LayoutElement>().flexibleWidth = 1f;

        var fill = CreateUI("Fill", back.transform);
        var fillImage = fill.AddComponent<Image>();
        fillImage.color = color;
        fillImage.raycastTarget = false;
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        return fillRect;
    }

    static TextMeshProUGUI CreateText(string name, Transform parent, string text, float size)
    {
        var go = CreateUI(name, parent);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = Color.black;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        return tmp;
    }

    static GameObject CreateUI(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, false);
        return go;
    }

    static void Stretch(RectTransform rect, float padding)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(padding, padding);
        rect.offsetMax = new Vector2(-padding, -padding);
    }

    static void ClearGenerated(Transform panel)
    {
        for (int i = panel.childCount - 1; i >= 0; i--)
        {
            var child = panel.GetChild(i);
            if (GeneratedNames.Contains(child.name)) Undo.DestroyObjectImmediate(child.gameObject);
        }
    }

    static List<T> LoadAll<T>() where T : Object
    {
        return AssetDatabase.FindAssets("t:" + typeof(T).Name)
            .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(a => a != null)
            .ToList();
    }
}
