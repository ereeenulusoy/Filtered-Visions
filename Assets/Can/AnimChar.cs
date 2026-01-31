using UnityEngine;

public class AnimChar : MonoBehaviour
{
   
    public float speed; 
    public float JumpForce;
    private Rigidbody rb;
    private Animator anim;

    private bool isGrounded;

    private void Start()
    {
         rb = GetComponent<Rigidbody>(); 
        anim = GetComponent<Animator>();

    }

  private void   FixedUpdate()
    {
        float yatay = Input.GetAxis("Horizontal"); // A-D veya Sol-Sað ok
        float dikey = Input.GetAxis("Vertical");   // W-S veya Yukarý-Aþaðý ok

        Vector3 moveForce = new Vector3(yatay, 0, dikey) * speed  * Time.deltaTime;
        transform.Translate(moveForce);

        bool isMoving = moveForce.magnitude > 0;
        if (isGrounded)
            anim.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            isGrounded = false;
            anim.SetBool("isJumping", true);
        }
    } 


    // Yere deðdiðini anlamak için basit çarpýþma kontrolü
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            // Düþme animasyonunu bitirip Idle/Walk'a dönmek için burasý kullanýlýr
        }
    }
}
