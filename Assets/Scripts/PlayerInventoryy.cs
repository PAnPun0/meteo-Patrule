using UnityEngine;
using TMPro; // Для красивого текста TextMeshPro

public class PlayerInventory : MonoBehaviour
{
    public int wool;       // Шерсть
    public int fleece;     // Флис
    public int membrane;   // Мембрана
    public bool hasArcticParka = false; // Появится после крафта
    public bool hasWarmJacket = false;  // Куртка для замёрзшего NPC

    public TextMeshProUGUI woolText;
    public TextMeshProUGUI fleeceText;
    public TextMeshProUGUI membraneText;

    void Start()
    {
        // Подтягиваем материалы из прошлой сцены
        if (GameSession.hasInventoryData)
        {
            wool = GameSession.wool;
            fleece = GameSession.fleece;
            membrane = GameSession.membrane;
            hasArcticParka = GameSession.hasArcticParka;
            hasWarmJacket = GameSession.hasWarmJacket;
        }
        UpdateUI();
    }

    void OnDestroy()
    {
        // Сохраняем материалы при выходе из сцены
        GameSession.hasInventoryData = true;
        GameSession.wool = wool;
        GameSession.fleece = fleece;
        GameSession.membrane = membrane;
        GameSession.hasArcticParka = hasArcticParka;
        GameSession.hasWarmJacket = hasWarmJacket;
    }

    public void AddMaterial(string type, int amount)
    {
        if (type == "Wool") wool += amount;
        else if (type == "Fleece") fleece += amount;
        else if (type == "Membrane") membrane += amount;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (woolText) woolText.text = "Шерсть: " + wool;
        if (fleeceText) fleeceText.text = "Флис: " + fleece;
        if (membraneText) membraneText.text = "Мембрана: " + membrane;
    }
}