using UnityEngine;

public class LocalRotate : MonoBehaviour
{
   
    public float speed = 1f; // Döndürme hýzý
    void Update()
    {
        // Yerel eksenlerde döndürme
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }
}
