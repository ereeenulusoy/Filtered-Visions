using UnityEngine;
using UnityEngine.Video; // Video kütüphanesini ekledik
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Video Ayarlarý")]
    public VideoPlayer introVideo; // Inspector panelinden Video Player'ý buraya sürükle

    public void StartGame()
    {
        // Eðer video atanmýþsa videoyu baþlat ve bitiþini bekle
        if (introVideo != null)
        {
            introVideo.Play();
            introVideo.loopPointReached += ChangeScene; // Video bitince ChangeScene çalýþýr
        }
        else
        {
            // Video yoksa direkt sahneye geç
            ChangeScene(null);
        }
    }

    // Video bittiðinde çaðrýlacak metod
    private void ChangeScene(VideoPlayer vp)
    {
        SceneManager.LoadScene("CAnd");
    }
}