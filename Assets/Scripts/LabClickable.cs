using UnityEngine;
using UnityEngine.EventSystems;

// Объект в лаборатории (верстак, шкаф), который открывает свою панель по клику мышью.
// Нужен Collider2D на объекте и камера с тегом MainCamera.
[RequireComponent(typeof(Collider2D))]
public class LabClickable : MonoBehaviour
{
    [Tooltip("Панель на Canvas, которая открывается по клику (по умолчанию выключена)")]
    public GameObject panel;

    [Tooltip("Необязательно: подсказка при наведении, например \"Верстак\"")]
    public GameObject hintText;

    [Header("Подсветка при наведении")]
    public Color hoverColor = new Color(1f, 1f, 0.75f);

    // Открытая сейчас панель — одновременно открыта только одна
    private static LabClickable current;

    private SpriteRenderer spriteRenderer;
    private Color normalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) normalColor = spriteRenderer.color;
        if (hintText) hintText.SetActive(false);
        if (panel) panel.SetActive(false);
    }

    void Update()
    {
        if (current == this && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    void OnMouseEnter()
    {
        if (IsPointerOverUI()) return;
        if (spriteRenderer) spriteRenderer.color = hoverColor;
        if (hintText) hintText.SetActive(true);
    }

    void OnMouseExit()
    {
        if (spriteRenderer) spriteRenderer.color = normalColor;
        if (hintText) hintText.SetActive(false);
    }

    void OnMouseDown()
    {
        // Клик по открытой панели не должен "проваливаться" на объект под ней
        if (IsPointerOverUI()) return;

        if (current == this)
        {
            Close();
            return;
        }

        if (current != null) current.Close();
        Open();
    }

    public void Open()
    {
        current = this;
        if (panel) panel.SetActive(true);
    }

    // Можно повесить на кнопку "Закрыть" в панели
    public void Close()
    {
        if (panel) panel.SetActive(false);
        if (current == this) current = null;
    }

    void OnDestroy()
    {
        if (current == this) current = null;
    }

    static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
