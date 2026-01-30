using UnityEngine;

public class Deneme : MonoBehaviour
{

    public GameObject RedFiltered;
    public GameObject blueFiltered;

    public GameObject[] blueBox;
    public GameObject[] redBox;

    private void Start()
    {
         ActivateBlueFilter (); 
    }

    private void Update()
    {
         if(Input.GetKeyDown(KeyCode.Q))
         {
             ActivateRedFilter();

        }
         else if(Input.GetKeyDown(KeyCode.E))
         {
           
            ActivateBlueFilter(); 
        }
    }

    public void ActivateRedFilter()
    {
        RedFiltered.SetActive(true);
        blueFiltered.SetActive(false);
        for (int i = 0; i < redBox.Length; i++)
        {
            redBox[i].SetActive(true);

        }
        for (int i = 0; i < blueBox.Length; i++)
        {
            blueBox[i].SetActive(false);
        }
    }

    public void ActivateBlueFilter()
    {
        RedFiltered.SetActive(false);
        blueFiltered.SetActive(true);

        for (int i = 0; i < redBox.Length; i++)
        {
            redBox[i].SetActive(false);

        }
        for (int i = 0; i < blueBox.Length; i++)
        {
            blueBox[i].SetActive(true);
        }
    }
}
