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
    public float airControlSpeed = 20f;

    [Header("Ground Check")]
    public bool isGrounded;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    // --- HİBRİT KONTROL ---
    private bool usePhysicsMovement = false;
    [HideInInspector] public float jumpCooldownTimer = 0f;

    private int jumpCount = 0;
    private float yposition;
    private Color FadeColorAlpha;

    // Hash ID'leri
    private int inputXHash = Animator.StringToHash("inputX");
    private int inputYHash = Animator.StringToHash("inputY");
    private int isGroundedHash = Animator.StringToHash("isGrounded");
    private int isJumpingHash = Animator.StringToHash("isJumping");
    private int isFallingHash = Animator.StringToHash("isFalling");

    // --- YENİ EKLENEN ANİMASYON HASHLERİ ---
    private int isWallRunningHash = Animator.StringToHash("isWallRunning");
    private int wallSideHash = Animator.StringToHash("wallSide");
    private int trigWallJumpHash = Animator.StringToHash("trigWallJump");

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        wallRunSystem = GetComponent<WallRunningSystem>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (anim == null) Debug.LogError("Animator bulunamadı!");
        cam = GetComponentInChildren<Camera>();

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
        if (jumpCooldownTimer > 0) jumpCooldownTimer -= Time.deltaTime;

        if ((wallRunSystem != null && wallRunSystem.IsWallJumpPossible()) || jumpCooldownTimer > 0) return;

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 1)
        {
            Jump();
        }

        yposition = transform.position.y;
        if (yposition < -10f) Respawn();
        FadeInOut();
    }

    private void FixedUpdate()
    {
        if (wallRunSystem != null && wallRunSystem.isWallRunning)
        {
            isGrounded = false;
            UpdateAnimation(0, 0);
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // --- HİBRİT HAREKET ---
        if (usePhysicsMovement)
        {
            // FİZİK MODU (Wall Jump sonrası)
            Vector3 camForward = cam.transform.forward; camForward.y = 0; camForward.Normalize();
            Vector3 camRight = cam.transform.right; camRight.y = 0; camRight.Normalize();
            Vector3 airMove = (camForward * vertical + camRight * horizontal).normalized * airControlSpeed;

            rb.AddForce(airMove, ForceMode.Acceleration);
        }
        else
        {
            // NORMAL MOD (TRANSLATE)
            Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
            Vector3 worldDir = transform.TransformDirection(movement.normalized);

            if (movement.magnitude > 0.001f)
            {
                if (!rb.SweepTest(worldDir, out RaycastHit hit, movement.magnitude + 0.01f))
                {
                    transform.Translate(movement);
                }
            }
        }

        Quaternion targetRotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 15f * Time.deltaTime);

        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
            if (usePhysicsMovement) StopPhysicsMomentum(); // Yere değersek hemen kes
        }

        UpdateAnimation(horizontal, vertical);
    }

    public void EnablePhysicsMovement()
    {
        usePhysicsMovement = true;
    }

    public void StopPhysicsMomentum()
    {
        if (!usePhysicsMovement) return;

        usePhysicsMovement = false;
        // Yatay hızı sıfırla, Gravity kalsın
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    public void LockJumpInput(float duration)
    {
        jumpCooldownTimer = duration;
    }

    public void ResetJumpCount()
    {
        jumpCount = 0;
    }

    // --- YENİ EKLENEN: DUVAR ANİMASYONU ---
    public void SetWallRunAnimation(bool isRunning, float side)
    {
        if (anim == null) return;

        anim.SetBool(isWallRunningHash, isRunning);

        // Eğer koşuyorsak tarafı güncelle (-1 Sol, 1 Sağ)
        if (isRunning)
        {
            float currentSide = anim.GetFloat(wallSideHash);
            // Lerp ile yumuşak geçiş
            anim.SetFloat(wallSideHash, Mathf.Lerp(currentSide, side, 10f * Time.deltaTime));
        }
    }

    // --- YENİ EKLENEN: DUVAR ZIPLAMASI ---
    public void TriggerWallJumpAnimation()
    {
        if (anim == null) return;
        anim.SetTrigger(trigWallJumpHash);
    }

    private void UpdateAnimation(float x, float y)
    {
        if (anim == null) return;
        float currentX = anim.GetFloat(inputXHash);
        float currentY = anim.GetFloat(inputYHash);
        anim.SetFloat(inputXHash, Mathf.Lerp(currentX, x, 0.25f));
        anim.SetFloat(inputYHash, Mathf.Lerp(currentY, y, 0.25f));
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
        jumpCount = 0;
        usePhysicsMovement = false;
        jumpCooldownTimer = 0;
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