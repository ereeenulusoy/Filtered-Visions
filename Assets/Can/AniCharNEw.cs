using UnityEngine;
using UnityEngine.UI;

public class AniCharNEw : MonoBehaviour
{
    [Header("References")]
    public Transform ResPoint;
    private Animator anim;
    private Rigidbody rb;
    public Camera cam;
    public Image fadeImage;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public bool isGrounded;
    public float groundCheckDistance = 0.2f; // Raycast mesafesi
    public LayerMask groundLayer; // Inspector'dan Ground Layer seçilmeli!

    // Private Variables
    private int jumpCount = 0;
    private float yposition;
    private Color FadeColorAlpha;

    // Animator Hash ID'leri (Performans için)
    private int inputXHash = Animator.StringToHash("inputX");
    private int inputYHash = Animator.StringToHash("inputY");
    private int isGroundedHash = Animator.StringToHash("isGrounded");
    private int isJumpingHash = Animator.StringToHash("isJumping");
    private int isFallingHash = Animator.StringToHash("isFalling");

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Animator'ı çocuk objelerde ara (senin hiyerarşine uygun)
        anim = GetComponentInChildren<Animator>();

        if (anim == null) Debug.LogError("Animator bulunamadı!");

        cam = GetComponentInChildren<Camera>();

        // Fade işlemleri
        fadeImage = FindFirstObjectByType<Image>();
        if (fadeImage != null)
        {
            FadeColorAlpha = fadeImage.color;
            FadeColorAlpha.a = 0f;
            fadeImage.color = FadeColorAlpha;
        }
    }

    private void Update()
    {
        // 1. INPUT ALMA
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 1)
        {
            Jump();
        }

        // 2. FADE KONTROLÜ
        yposition = transform.position.y;
        if (yposition < -10f) Respawn();
        FadeInOut();
    }

    private void FixedUpdate()
    {
        // --- 1. HAREKET (Arkadaşının Mantığı Korundu) ---
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Kameraya göre hareket yönü
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
        transform.Translate(movement);

        // Karakteri hep kameranın baktığı yöne çevir
        transform.localRotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);

        // --- 2. GROUND CHECK (Collision yerine Raycast - ÇOK ÖNEMLİ) ---
        // OnCollisionEnter animasyonlar için yetersiz kalır (Falling için).
        // Karakterin hafif üstünden aşağıya ışın atıyoruz.
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
        }

        // --- 3. ANIMASYON GÜNCELLEME ---
        UpdateAnimation(horizontal, vertical);
    }

    private void UpdateAnimation(float x, float y)
    {
        if (anim == null) return;

       
        float currentX = anim.GetFloat(inputXHash);
        float currentZ = anim.GetFloat(inputYHash);

        anim.SetFloat(inputXHash, Mathf.Lerp(currentX, x, 0.25f));
        anim.SetFloat(inputYHash, Mathf.Lerp(currentZ, y, 0.25f));

        
        anim.SetBool(isGroundedHash, isGrounded);

       
        bool falling = !isGrounded && rb.linearVelocity.y < -0.1f;
        bool jumping = !isGrounded && rb.linearVelocity.y > 0.1f;

        anim.SetBool(isFallingHash, falling);
        anim.SetBool(isJumpingHash, jumping);
    }

    private void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

       
        jumpCount++;
    }

    private void Respawn()
    {
        transform.position = ResPoint.position;
        
        rb.linearVelocity = Vector3.zero;
    }

    private void FadeInOut()
    {
        if (fadeImage == null) return;

        if (yposition < 0f)
        {
            float fadeDuration = yposition / -10f;
            FadeColorAlpha.a = fadeDuration;
            fadeImage.color = FadeColorAlpha;
        }
        else
        {
            FadeColorAlpha.a = 0f;
            fadeImage.color = FadeColorAlpha;
        }
    }

   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.up * 0.1f + Vector3.down * groundCheckDistance);
    }
}