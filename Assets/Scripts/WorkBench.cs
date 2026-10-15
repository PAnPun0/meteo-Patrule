using UnityEngine;

public class Workbench : MonoBehaviour
{
    [Header("Радиус взаимодействия")]
    public GameObject hintText; // "[E] Верстак"
    public GameObject craftingUIPanel; // Ссылка на панель крафта в Canvas

    private bool playerInRange = false;

    void Start()
    {
        if (hintText) hintText.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Переключаем видимость меню крафта
            bool state = !craftingUIPanel.activeSelf;
            craftingUIPanel.SetActive(state);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (hintText) hintText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (hintText) hintText.SetActive(false);
            if (craftingUIPanel) craftingUIPanel.SetActive(false); // Закрываем при уходе
        }
    }
}