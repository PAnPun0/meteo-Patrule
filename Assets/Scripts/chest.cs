using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum MaterialType { Wool, Fleece, Membrane }

    [Header("Что лежит внутри")]
    public MaterialType dropType;
    public int amount = 1;

    [Header("Визуал")]
    public GameObject hintText; // Подсказка "[E] Открыть"
    public Sprite openedChestSprite; // Спрайт открытого сундука (необязательно)

    private bool isPlayerNearby = false;
    private bool isOpened = false;
    private PlayerInventory playerInventory;

    void Start()
    {
        if (hintText != null) hintText.SetActive(false);
    }

    void Update()
    {
        // Если игрок рядом, сундук закрыт и нажата 'E'
        if (isPlayerNearby && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        if (hintText != null) hintText.SetActive(false);

        // Отдаем ресурс игроку
        playerInventory.AddMaterial(dropType.ToString(), amount);

        // Меняем спрайт на открытый или меняем цвет
        if (openedChestSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = openedChestSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }

        Debug.Log($"Получено: {dropType} x{amount}!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpened && other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            if (hintText != null) hintText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (hintText != null) hintText.SetActive(false);
        }
    }
}