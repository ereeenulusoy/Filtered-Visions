using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Tooltip("Skybox'ýn dönüþ hýzý. Yavaþ bir etki için düþük deðerler (örn: 0.5 veya 1) kullanýn.")]
    public float rotationSpeed = 1.0f;


    private void Start()
    {
       Application.targetFrameRate = 144;
    }
    void Update()
    {

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}