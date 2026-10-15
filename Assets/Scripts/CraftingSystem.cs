using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public PlayerInventory inventory;
    public PlayerColdSystem coldSystem;
    public SpriteRenderer playerVisual; // Чтобы перекрасить персонажа в цвет новой куртки

    [Header("Кнопка крафта")]
    public Button craftArcticParkaButton;

    // Рецепт 1: Арктическая парка (Нужно: 2 Шерсти + 1 Мембрана)
    public void CraftArcticParka()
    {
        if (inventory.wool >= 2 && inventory.membrane >= 1)
        {
            // 1. Тратим ресурсы
            inventory.wool -= 2;
            inventory.membrane -= 1;
            inventory.UpdateUI();
            coldSystem.EquipClothing(0.8f);

            // 3. Визуальный апгрейд — красим куртку в арктический синий
            if (playerVisual != null)
            {
                playerVisual.color = new Color(0.2f, 0.7f, 1f); 
            }

            // Отключаем кнопку, чтобы не крафтить дважды
            craftArcticParkaButton.interactable = false;

            Debug.Log("Скрафчена Арктическая Парка!");
        }
        else
        {
            Debug.Log("Недостаточно ресурсов!");
        }
        inventory.hasArcticParka = true;
    }
}