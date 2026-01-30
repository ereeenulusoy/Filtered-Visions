using UnityEngine;
using UnityEngine.UI;

public class Playertrial : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody rb; 

    private float yposition; 
    public Transform ResPoint;

    private float fadeDuration = 0f; 
    public Image fadeImage;
    private Color FadeColorAlpha;


    private void Start()
    {
         rb = GetComponent<Rigidbody>(); 
        Debug.Log(ResPoint.position);
        // Eðer fadeImage'i Inspector'dan atýyorsanýz þu satýrý kaldýrabilirsiniz.
        fadeImage = FindAnyObjectByType<Image>();
        // Hata: 'fadeImage.color.a = 0f;' yazýlamaz çünkü 'color' bir struct (Color) olarak deðere göre döner.
        // Dönen struct'ýn alanýný doðrudan deðiþtiremezsiniz. Bunun yerine Color'u alýp deðiþtirip tekrar atayýn:
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key was pressed.");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
        }

        yposition = transform.position.y;

        if (yposition < -10f)
        {
            Respawn();
        }

        FadeInOut();


    }

    private void Respawn()
    {
        // 19.13, 0.50, -11.04
        transform.position = ResPoint.position;
        rb.linearVelocity = Vector3.zero;
    } 

    private void FadeInOut()
    {
        // Implement fade in/out effect here

        if (yposition < 0f)
        {
            fadeDuration = yposition/-10f;
          
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


}
