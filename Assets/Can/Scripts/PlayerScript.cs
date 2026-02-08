using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private float yposition;
    public Transform ResPoint;

    public float speed = 5f;
    public float jumpFOrce = 5f;
    public int jumpCount = 0;

    private float fadeDuration = 0f;
    public Image fadeImage;
    private Color FadeColorAlpha;

    private Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log(ResPoint.position);
        fadeImage = FindFirstObjectByType<Image>();
        if (fadeImage != null)
        {
            FadeColorAlpha = fadeImage.color;
            FadeColorAlpha.a = 0f;
            fadeImage.color = FadeColorAlpha;
        }
        else
        {
            Debug.LogWarning("fadeImage null. Inspector'dan atayýn veya bir Image bileþeni ekleyin.");
        }

    }

    private void Update()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
        transform.Translate(movement);

        transform.localRotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);


        yposition = transform.position.y;

        if (yposition < -10f)
        {
            Respawn();
        }

        FadeInOut();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            rb.AddForce(Vector3.up * jumpFOrce, ForceMode.Impulse);
            jumpCount++;
        }

    }

    private void Respawn()
    {
        transform.position = ResPoint.position;

    }

    private void FadeInOut()
    {

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
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }


}
