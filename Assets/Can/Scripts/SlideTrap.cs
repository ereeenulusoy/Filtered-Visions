using UnityEngine;

public class SlideTrap : MonoBehaviour
{

    public float trapLimit = -5f;
    private Vector3 offset;

        
        private Transform Transform; 

    private void Start()
    {
         offset = transform.position; 
    }



    public void goForward()
    {
               if (transform.position.z < trapLimit)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 5f);
        }

    }

    public void goBack()
    {
        if (transform.position.z > offset.z)
        {
            transform.Translate(Vector3.back * Time.deltaTime * 5f);
        }
    }



    }
