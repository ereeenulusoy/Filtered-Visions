using UnityEngine;

public class Downtrap : MonoBehaviour
{
    private bool isActive = false ;
    private float trapPosY;


    private void Start()
    {
         isActive = false; 
        trapPosY = transform.position.y;
    }

    private void FixedUpdate()
    {
         if(isActive == true)
         {
          trapActive();
        }

        if (isActive == false )
        {
            trapReset();
        }

      
    }

    private void OnCollisionEnter(Collision collision)
    {    
            isActive = true;
    }

  

    public void trapActive()
    {
        float timer = Time.deltaTime; 

        if (timer < 4f)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 2f);
        }

        if(timer >= 4f)
        {
            isActive = false;
            timer = 0f;
        }

    }

    public void trapReset()
    {

        if (transform.position.y < trapPosY)
        {
           
            transform.Translate(Vector3.up * Time.deltaTime * 2f);
        }
    }

    
}
