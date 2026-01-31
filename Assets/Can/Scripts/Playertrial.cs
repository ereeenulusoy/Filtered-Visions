using UnityEngine;
using UnityEngine.UI;

public class Playertrial : MonoBehaviour
{
    
  
    private float yposition; 
    public Transform ResPoint;

    private float fadeDuration = 0f; 
    public Image fadeImage;
    private Color FadeColorAlpha;



    private void Start()
    {
         
        Debug.Log(ResPoint.position);
        // Eðer fadeImage'i Inspector'dan atýyorsanýz þu satýrý kaldýrabilirsiniz.
        fadeImage = FindFirstObjectByType<Image>();
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

