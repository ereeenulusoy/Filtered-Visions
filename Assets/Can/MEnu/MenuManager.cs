using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO; // Path iþlemleri için gerekli

public class MenuManager : MonoBehaviour
{
    [Header("Video Ayarlarý")]
    public VideoPlayer introVideo; // Video Player objesini buraya sürükle
    public string videoFileName = "intro.mp4"; // StreamingAssets içindeki dosya adý
    public GameObject videoScreenObject; // Video Player'ýn olduðu GameObject (Kendisi)

    public void StartGame()
    {
        // Video atanmýþsa süreci baþlat
        if (introVideo != null && videoScreenObject != null)
        {
            // 1. Önce videonun olduðu objeyi aç (Görünür yap)
            videoScreenObject.SetActive(true);

            // 2. WebGL için URL yolunu ayarla (StreamingAssets mantýðý)
            string videoPath = Path.Combine(Application.streamingAssetsPath, videoFileName);
            introVideo.source = VideoSource.Url;
            introVideo.url = videoPath;

            // 3. Videoyu hazýrla ve olaylarý dinle
            introVideo.prepareCompleted += OnVideoPrepared; // Hazýr olunca ne yapacaðýný söyle
            introVideo.loopPointReached += ChangeScene;     // Bitince ne yapacaðýný söyle

            introVideo.Prepare(); // Hazýrlamaya baþla
        }
        else
        {
            // Video yoksa direkt oyuna gir
            ChangeScene(null);
        }
    }

    // Video belleðe yüklenip hazýr olduðunda çalýþýr
    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play(); // Hazýr olunca OYNAT
    }

    // Video bittiðinde çalýþýr
    private void ChangeScene(VideoPlayer vp)
    {
        // Olay aboneliklerini temizle (Hata almamak için)
        vp.loopPointReached -= ChangeScene;
        vp.prepareCompleted -= OnVideoPrepared;

        SceneManager.LoadScene("CAnd");
    }
}