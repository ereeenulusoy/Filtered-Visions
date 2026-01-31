using UnityEngine;

public class UpSlide : MonoBehaviour
{

    private Vector3 closedPos;
    public float speed = 2f;
    bool isgoing = false;
    float timer;

    private void Start()
    {
        closedPos = transform.position;
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
            transform.Translate(Vector3.up * Time.deltaTime * speed);
        }

        else if (!isgoing)
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }

    }
}
