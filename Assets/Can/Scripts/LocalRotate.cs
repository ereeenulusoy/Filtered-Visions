using UnityEngine;

public class LocalRotate : MonoBehaviour
{
   
    public float speed = 1f; 
    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }
}
