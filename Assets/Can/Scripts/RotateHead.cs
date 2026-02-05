using UnityEngine;

public class RotateHead : MonoBehaviour
{
    [Header("Ayarlar")]
    public float rotationSpeed = 10f;

    private float targetY = 0f;

    void Start()
    {
       
        targetY = transform.localEulerAngles.y;
    }

    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetY = 180f;
        }

        
        if (Input.GetKeyDown(KeyCode.E))
        {
            targetY = 0f;
        }

       
        Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}