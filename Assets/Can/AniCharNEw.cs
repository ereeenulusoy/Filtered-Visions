using UnityEngine;
using UnityEngine.UI;

public class AniCharNEw : MonoBehaviour
{
    private WallRunningSystem wallRunSystem;

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
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    // Private Variables
    private int jumpCount = 0;
    private float yposition;
    private Color FadeColorAlpha;

    // Animator Hash ID'leri
    private int inputXHash = Animator.StringToHash("inputX");
    private int inputYHash = Animator.StringToHash("inputY");
    private int isGroundedHash = Animator.StringToHash("isGrounded");
    private int isJumpingHash = Animator.StringToHash("isJumping");
    private int isFallingHash = Animator.StringToHash("isFalling");

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        wallRunSystem = GetComponent<WallRunningSystem>();

        // TİTREME ÖNLEYİCİ #1: Rigidbody Ayarları
        rb.freezeRotation = true; // Fizik motoru karakteri döndürmesin
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Kareler arası yumuşatma

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
            // Duvardaysak zıplamayı WallRunSystem halletsin
            if (wallRunSystem != null && wallRunSystem.isWallRunning) return;

            Jump();
        }

        // 2. FADE KONTROLÜ
        yposition = transform.position.y;
        if (yposition < -10f) Respawn();
        FadeInOut();
    }

    private void FixedUpdate()
    {
        // --- WALL RUN KONTROLÜ ---
        if (wallRunSystem != null && wallRunSystem.isWallRunning)
        {
            isGrounded = false;
            UpdateAnimation(0, 0);
            // Duvardayken Translate yapmıyoruz, kontrol tamamen WallRunSystem'de (Fizik tabanlı)
            return;
        }

        // --- 1. NORMAL HAREKET (Translate ile) ---
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Hareket vektörü (Local Space)
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;

        // TİTREME ÖNLEYİCİ #2: Akıllı Hareket (Smart Move)
        // Translate yapmadan önce, gideceğimiz yönde duvar var mı diye kontrol ediyoruz.
        // Rigidbody.SweepTest bizim yerimize fiziği kontrol eder.
        // TransformDirection ile local hareketi dünya yönüne çeviriyoruz.
        Vector3 worldDir = transform.TransformDirection(movement.normalized);

        // Eğer gideceğimiz yönde (hareket miktarı kadar) bir engel YOKSA hareket et
        // Veya hareket çok küçükse (duruyorsak) kontrol etme
        if (movement.magnitude > 0.001f)
        {
            // Önümüzde engel yoksa yürü
            if (!rb.SweepTest(worldDir, out RaycastHit hit, movement.magnitude + 0.01f))
            {
                transform.Translate(movement);
            }
            // Engel varsa ama çok dik değilse (merdiven/yokuş gibi) yine de yürü
            else if (isGrounded)
            {
                // Basit bir kaydırma mantığı (duvara takılmamak için)
                // Burayı boş bırakıyoruz, SweepTest duvara girmeyi engellediği için titreme kesilir.
            }
        }

        // Karakteri hep kameranın baktığı yöne çevir
        // Slerp ekleyerek dönüşü biraz yumuşattım (Jitter azaltır)
        Quaternion targetRotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 15f * Time.deltaTime);

        // --- 2. GROUND CHECK ---
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
        float currentY = anim.GetFloat(inputYHash);

        // 0.25f yumuşatma iyidir
        anim.SetFloat(inputXHash, Mathf.Lerp(currentX, x, 0.25f));
        anim.SetFloat(inputYHash, Mathf.Lerp(currentY, y, 0.25f));

        anim.SetBool(isGroundedHash, isGrounded);

        // Unity 6 için linearVelocity, eski ise velocity
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
        // ... (Senin fade kodun aynı kalıyor)
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