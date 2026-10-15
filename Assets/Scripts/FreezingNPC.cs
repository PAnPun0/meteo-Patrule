using UnityEngine;

public class FreezingNPC : MonoBehaviour
{
    [Header("Спрайты состояний")]
    public Sprite coldSprite;   // npc1
    public Sprite warmSprite;   // npc2

    [Header("Эффект холода (ne.png, 2 кадра)")]
    public Sprite[] shiverFrames; // ne_0, ne_1
    public SpriteRenderer shiverRenderer; 
    public float shiverFrameTime = 0.15f;
    public float shakeAmount = 0.03f;
    public float shakeSpeed = 25f;

    [Header("Взаимодействие")]
    public GameObject hintText; // "[E] Отдать куртку"

    private bool playerInRange = false;
    private bool isWarm = false;
    private PlayerInventory playerInventory;
    private SpriteRenderer bodyRenderer;
    private Vector3 basePosition;
    private float shiverTimer;
    private int shiverFrameIndex;

    void Start()
    {
        bodyRenderer = GetComponent<SpriteRenderer>();
        basePosition = transform.localPosition;
        bodyRenderer.sprite = coldSprite;
        if (hintText) hintText.SetActive(false);
        if (shiverRenderer) shiverRenderer.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isWarm)
        {
            float offsetX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeAmount;
            transform.localPosition = basePosition + new Vector3(offsetX, 0f, 0f);

            if (shiverRenderer != null && shiverFrames.Length > 0)
            {
                shiverTimer += Time.deltaTime;
                if (shiverTimer >= shiverFrameTime)
                {
                    shiverTimer = 0f;
                    shiverFrameIndex = (shiverFrameIndex + 1) % shiverFrames.Length;
                    shiverRenderer.sprite = shiverFrames[shiverFrameIndex];
                }
            }
        }

        if (playerInRange && !isWarm && Input.GetKeyDown(KeyCode.E))
        {
            TryGiveJacket();
        }
    }

    void TryGiveJacket()
    {
        if (playerInventory == null || !playerInventory.hasWarmJacket) return;

        playerInventory.hasWarmJacket = false;
        WarmUp();
    }

    void WarmUp()
    {
        isWarm = true;
        transform.localPosition = basePosition;
        bodyRenderer.sprite = warmSprite;
        if (shiverRenderer) shiverRenderer.gameObject.SetActive(false);
        if (hintText) hintText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isWarm && other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            if (hintText) hintText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (hintText) hintText.SetActive(false);
        }
    }
}
