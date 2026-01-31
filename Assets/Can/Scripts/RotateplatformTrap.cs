using UnityEngine;

public class RotateplatformTrap : MonoBehaviour
{
  
    private Transform playerTransform;
    private Quaternion initialRotation;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            // Karakterin o anki dünya rotasyonunu kaydet
            initialRotation = playerTransform.rotation;

            // Karakteri platformun çocuðu yap
            playerTransform.SetParent(this.transform);
        }
    }

    private void LateUpdate()
    {
        // Eðer karakter platformun üzerindeyse, her frame rotasyonunu sabitle
        if (playerTransform != null && playerTransform.parent == this.transform)
        {
            // Platform dönse bile karakterin rotasyonunu baþlangýçtaki halinde tutar
            playerTransform.rotation = initialRotation;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            playerTransform = null;
        }
    }
}

