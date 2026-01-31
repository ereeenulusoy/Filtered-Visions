using System.Threading;
using UnityEngine;

public class slideTRAP1 : MonoBehaviour
{
    public Transform StartPoint;

    private Vector3 closedPos; 
    public float speed = 2f;
    bool isgoing = false;
    float timer;

    private void Start()
    {
         closedPos = transform.position;
        transform.position = StartPoint.position;
    }

    private void Update()
    {
         timer = timer + Time.deltaTime;
       


        if (timer > 2f)
        {
            isgoing = !isgoing;
            timer = 0f;

        }
        if (isgoing)
        {
           transform.Translate(Vector3.right * Time.deltaTime * speed);
        }

        else if (!isgoing)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }

    }
}
