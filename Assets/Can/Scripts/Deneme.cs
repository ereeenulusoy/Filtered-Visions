using UnityEngine;

public class Deneme : MonoBehaviour
{

    public GameObject RedFiltered;
    public GameObject blueFiltered;

    public GameObject[] blueBox;
    public GameObject[] redBox;

    private void Update()
    {
         if(Input.GetKeyDown(KeyCode.Q))
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
         else if(Input.GetKeyDown(KeyCode.E))
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
}
