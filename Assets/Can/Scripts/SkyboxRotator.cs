using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Tooltip("Skybox'ýn dönüþ hýzý. Yavaþ bir etki için düþük deðerler (örn: 0.5 veya 1) kullanýn.")]
    public float rotationSpeed = 1.0f;

    void Update()
    {
        // RenderSettings üzerinden mevcut Skybox materyaline eriþip Rotation deðerini deðiþtirir
        // Time.time kullanarak zamanla sürekli artan bir deðer elde ederiz.
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}