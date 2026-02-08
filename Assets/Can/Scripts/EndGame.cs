using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public GameObject endGameUI;
    public GameObject character;
    public GameObject endCamera;

    private void Start()
    {
        Application.targetFrameRate = 144;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            endGameUI.SetActive(true);

          
            character.SetActive(false);

            endCamera.SetActive(true);

            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void EndGameGoMenu()
    {
        
        SceneManager.LoadScene(0);
    }
}