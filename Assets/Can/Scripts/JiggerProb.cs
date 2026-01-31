using UnityEngine;

public class JiggerProb : MonoBehaviour
{
  
    private void OnCollisionEnter(Collision collision)
    {
        // Eðer çarpan obje "Player" ise (Tag'inin Player olduðundan emin ol)
        if (collision.gameObject.CompareTag("Player"))
        {
            // Karakteri platformun çocuðu yap
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Karakter platformdan inince baðý kopar
            collision.transform.SetParent(null);

            // Eðer "DontDestroyOnLoad" objen varsa null yerine ona ataman gerekebilir
            // Genelde null yeterlidir.
        }
    }
}

