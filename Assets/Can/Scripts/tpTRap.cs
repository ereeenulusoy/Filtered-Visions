using System.Collections;
using UnityEngine;

public class tpTRap : MonoBehaviour
{
    private Vector3 closedPos;
    public float distance = 2f;
    int tpState = 0; // 0 = closed, 1 = opening, 2 = open, 
    float timer;


    private void Start()
    {
        timer = 0f;
        tpState = 1;
        closedPos = transform.position;
        Debug.Log("Closed Position: " + closedPos);

    }


    private void Update()
    {
        timer = timer + Time.deltaTime;

        if (timer > 2f)
        {
            tpState++;
            timer = 0f;
            if (tpState > 2)
            {
                tpState = 0;
            }
        }

        switch (tpState)
        {
            case 0: transform.Translate(closedPos); break;
            case 1: transform.Translate(closedPos + new Vector3(0, 0, 2)); break;
            case 2: transform.Translate(closedPos + new Vector3(0, 0, 2)); break;



        }

    }
}
