using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AniCharNEw : MonoBehaviour
{

    private float yposition;
    public Transform ResPoint;
    private Animator anim;

    public float speed = 5f;
    public float jumpFOrce = 5f;
    public int jumpCount = 0;

    private float fadeDuration = 0f;
    public Image fadeImage;
    private Color FadeColorAlpha;

    private Rigidbody rb;
    public Camera cam;

    public bool isGrounded;
    public float groundCheckDistance = 0.1f;
    public bool isMoving;


    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.Log("ANİM NOT FOUND");
        }


        cam = GetComponentInChildren<Camera>();

        fadeImage = FindFirstObjectByType<Image>();

        if (fadeImage != null)
        {
            FadeColorAlpha = fadeImage.color;
            FadeColorAlpha.a = 0f;
            fadeImage.color = FadeColorAlpha;
        }
        else
        {
            Debug.LogWarning("fadeImage null. Inspector'dan atayın veya bir Image bileşeni ekleyin.");
        }

    }
    private void Update()
    {

        if (isGrounded)
        {
            jumpCount = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 1)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.up * jumpFOrce, ForceMode.Impulse);
            Debug.Log("JUMPED");
            jumpCount++;
        }
    }
    private void FixedUpdate()
    {
        
       // isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance);
        
       // Debug.Log("is grounded: " + isGrounded);




        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
        transform.Translate(movement);

        transform.localRotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);



        yposition = transform.position.y;

        if (yposition < -10f)
        {
            Respawn();
        }

        FadeInOut();
       

        if (horizontal != 0f || vertical != 0f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        


        anim.SetBool("isMoving", isMoving && isGrounded);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isIdling", !isMoving);
       anim.SetBool("isJumping", !isGrounded);


    }

    private void Respawn()
    {
        // 19.13, 0.50, -11.04
        transform.position = ResPoint.position;

    }


   

    private void FadeInOut()
    {
        // Implement fade in/out effect here

        if (yposition < 0f)
        {
            fadeDuration = yposition / -10f;

            FadeColorAlpha.a = fadeDuration;
            fadeImage.color = FadeColorAlpha;

        }
        else if (yposition >= 0f)
        {
            fadeDuration = 0f;
            FadeColorAlpha.a = fadeDuration;
            fadeImage.color = FadeColorAlpha;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }



}
