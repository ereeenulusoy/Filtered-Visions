using UnityEngine;

public class AnimChar : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public bool isGrounded;

    private Rigidbody rb;
    private Animator anim;
    private float yatay;
    private float dikey;
    private bool jumpRequested;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // Karakterin devrilmesini engellemek i�in (X ve Z rotasyonunu dondur)
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        // Girdi (Input) her zaman Update i�inde al�n�r
        yatay = Input.GetAxis("Horizontal");
        dikey = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }

        // Animasyon parametrelerini burada g�ncellemek daha ak�c�d�r
        bool isMoving = (Mathf.Abs(yatay) > 0.1f || Mathf.Abs(dikey) > 0.1f);
        anim.SetBool("isMoving", isMoving && isGrounded);
    }

    private void FixedUpdate()
    {
        // HAREKET: Translate yerine Velocity kullanarak yer�ekimine izin veriyoruz
        Vector3 moveDir = new Vector3(yatay, 0, dikey).normalized;
        Vector3 targetVelocity = moveDir * speed;

        // Y eksenindeki h�z� (yer�ekimini) koruyarak sadece X ve Z'yi de�i�tiriyoruz
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // ZIPLAMA
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpRequested = false;
            anim.SetBool("isJumping", true); // Animator'da bu parametre varsa
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Tag'in tam olarak "Ground" yaz�ld���ndan emin ol (B�y�k/k���k harf duyarl�)
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
    }
}