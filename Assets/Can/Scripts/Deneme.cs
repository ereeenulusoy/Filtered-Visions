using UnityEngine;

public class Deneme : MonoBehaviour
{

    public GameObject RedFiltered;
    public GameObject blueFiltered;

    public GameObject blueBox;
    public GameObject redBox;

    private void Update()
    {
         if(Input.GetKeyDown(KeyCode.Q))
         {
             RedFiltered.SetActive(true);
             blueFiltered.SetActive(false);
            blueBox.SetActive(false);
            redBox.SetActive(true);

        }
         else if(Input.GetKeyDown(KeyCode.E))
         {
             RedFiltered.SetActive(false);
             blueFiltered.SetActive(true);
            blueBox.SetActive(true);
            redBox.SetActive(false);
        }
    }
}
