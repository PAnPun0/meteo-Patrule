using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    private float horizontalInput;
    private bool isFacingRight = true;
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; 
    }

    void Update()
    {
        // На паузе не читаем управление
        if (PauseMenu.IsPaused)
        {
            horizontalInput = 0f;
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Прыжок
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        Flip();
        float facingDirection = isFacingRight ? 1f : -1f;
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            float scaleY = 1f + Mathf.Sin(Time.time * 12f) * 0.1f;
            float scaleX = (1f - Mathf.Sin(Time.time * 12f) * 0.1f) * facingDirection;
            transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
        else
        {
            Vector3 targetScale = new Vector3(facingDirection, 1f, 1f);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}