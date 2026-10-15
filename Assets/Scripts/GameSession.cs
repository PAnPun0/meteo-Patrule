// Данные, которые переживают переход между сценами (меню -> лаборатория -> локация).
// Статический класс не уничтожается при загрузке сцены.
public static class GameSession
{
    // Одежда, надетая в лаборатории
    public static EquipmentItem head;
    public static EquipmentItem torso;
    public static EquipmentItem legs;
    public static EquipmentItem feet;

    // Материалы инвентаря
    public static bool hasInventoryData = false;
    public static int wool;
    public static int fleece;
    public static int membrane;
    public static bool hasArcticParka;
    public static bool hasWarmJacket;

    // Вызывается кнопкой "Новая игра"
    public static void Reset()
    {
        head = null;
        torso = null;
        legs = null;
        feet = null;

        hasInventoryData = false;
        wool = 0;
        fleece = 0;
        membrane = 0;
        hasArcticParka = false;
        hasWarmJacket = false;
    }
}
