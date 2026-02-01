using UnityEngine;

public class WallRunningSystem : MonoBehaviour
{
    [Header("Ayarlar")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce = 200f; // Duvarda ilerleme hýzý
    public float wallJumpUpForce = 7f; // Yukarý zýplama gücü
    public float wallJumpSideForce = 12f; // Yana zýplama gücü
    public float maxWallRunTime = 1.5f; // Duvarda ne kadar kalabilir?

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection (Tespit)")]
    public float wallCheckDistance = 0.7f;
    public float minJumpHeight = 1.0f; // Yerden ne kadar yüksekte olmalý?

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Referanslar")]
    public Transform orientation; // Karakterin yönü (Kamera açýsý)
    private AniCharNEw mainScript; // Ana hareket scriptin
    private Rigidbody rb;

    // Durum Kontrolü
    public bool isWallRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainScript = GetComponent<AniCharNEw>();

        // Eðer Orientation atanmadýysa, kamerayý referans al (Geçici çözüm)
        if (orientation == null) orientation = mainScript.cam.transform;
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (isWallRunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        // Saðýmýza ve Solumuza ýþýn atýyoruz
        // orientation.right -> Sað taraf
        // -orientation.right -> Sol taraf
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        // Yerden yeterince yüksekte miyiz? (Raycast boyu: minJumpHeight)
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        // Inputlarý al
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Duvardaysak, W'ye basýyorsak ve Yerde Deðilsek -> Wall Run Baþlat
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!isWallRunning)
                StartWallRun();

            // Duvardayken Zýplama
            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
        }
        else
        {
            if (isWallRunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;

        // Ana scripte "Duvardayýz, yere basma kontrolünü veya yerçekimini sal" diyebiliriz
        // Þimdilik sadece yerçekimini kapatýyoruz.
        rb.useGravity = false;

        // Düþüþ hýzýný sýfýrla ki duvarda kaymasýn
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }

    private void WallRunningMovement()
    {
        // Duvarýn normalini (yüzey yönünü) al
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        // Duvarýn yönüne paralel bir "Ýleri" vektörü bul
        // (Matematiksel sihir: Normal ile Yukarý vektörünün çarpýmý duvarýn ileri yönünü verir)
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // Yönü düzelt (Karakterin baktýðý yöne çevir)
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // Ýleri Kuvvet Uygula
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Duvara Yapýþtýrma Kuvveti (Karakter duvardan kopmasýn diye hafifçe duvara itiyoruz)
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
    }

    private void WallJump()
    {
        // Önce wall run'ý bitir
        StopWallRun();

        // Zýplama Yönleri
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        // Yukarý + Duvarýn Tersi
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Hýzý sýfýrla ve fýrlat (Daha keskin zýplama için)
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}