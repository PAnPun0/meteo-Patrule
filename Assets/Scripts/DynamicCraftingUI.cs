using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicCraftingUI : MonoBehaviour
{
    [Header("Данные")]
    public List<CraftRecipe> allRecipes = new List<CraftRecipe>();
    public PlayerInventory playerInventory;
    public EquipmentSystem equipmentSystem;

    [Header("Префаб и контейнер")]
    public Transform recipeContainer; // Объект с LayoutGroup (Grid или Vertical)
    public GameObject recipeUIPrefab;  // Префаб плашки рецепта

    [Header("Описание вещи при наведении (необязательно)")]
    public ItemInfoPanel infoPanel;

    void OnEnable()
    {
        RefreshRecipeList();
    }

    public void RefreshRecipeList()
    {
        // Очищаем старые плашки
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаем плашку под каждый рецепт
        foreach (CraftRecipe recipe in allRecipes)
        {
            GameObject entry = Instantiate(recipeUIPrefab, recipeContainer);
            
            // Настройка текста и иконки рецепта
            entry.transform.Find("ItemIcon").GetComponent<Image>().sprite = recipe.resultItem.iconUI;
            entry.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = recipe.resultItem.itemName;

            // Короткие характеристики, если в префабе есть текст ItemStats
            var statsText = entry.transform.Find("ItemStats");
            if (statsText) statsText.GetComponent<TextMeshProUGUI>().text = recipe.resultItem.GetShortStatsText();
            ItemHoverInfo.Attach(entry, recipe.resultItem, infoPanel, GetIngredientsText(recipe));

            Button craftBtn = entry.transform.Find("CraftBtn").GetComponent<Button>();
            
            bool canCraft = CheckIngredients(recipe);
            craftBtn.interactable = canCraft;

            // Нажатие на кнопку создания
            craftBtn.onClick.AddListener(() => ExecuteCraft(recipe));
        }
    }

    private string GetIngredientsText(CraftRecipe recipe)
    {
        var parts = new List<string>();
        foreach (var ing in recipe.ingredients)
        {
            string name = ing.materialName == "Wool" ? "Шерсть"
                : ing.materialName == "Fleece" ? "Флис"
                : ing.materialName == "Membrane" ? "Мембрана"
                : ing.materialName;
            parts.Add($"{name} ×{ing.amount}");
        }
        return parts.Count > 0 ? "Нужно: " + string.Join(", ", parts) : "";
    }

    private bool CheckIngredients(CraftRecipe recipe)
    {
        foreach (var ing in recipe.ingredients)
        {
            if (ing.materialName == "Wool" && playerInventory.wool < ing.amount) return false;
            if (ing.materialName == "Membrane" && playerInventory.membrane < ing.amount) return false;
            if (ing.materialName == "Fleece" && playerInventory.fleece < ing.amount) return false;
        }
        return true;
    }

    private void ExecuteCraft(CraftRecipe recipe)
    {
        if (!CheckIngredients(recipe)) return;

        // 1. Списываем материалы
        foreach (var ing in recipe.ingredients)
        {
            if (ing.materialName == "Wool") playerInventory.wool -= ing.amount;
            if (ing.materialName == "Membrane") playerInventory.membrane -= ing.amount;
            if (ing.materialName == "Fleece") playerInventory.fleece -= ing.amount;
        }
        playerInventory.UpdateUI();

        // 2. Сразу автоматически экипируем созданный предмет в нужный слот
        equipmentSystem.Equip(recipe.resultItem);

        // 3. Обновляем список (кнопки пересчитаются по остатку ресурсов)
        RefreshRecipeList();
    }
}