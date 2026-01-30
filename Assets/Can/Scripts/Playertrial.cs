using UnityEngine;

public class Playertrial : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody rb; 

    private void Start()
    {
         rb = GetComponent<Rigidbody>(); 
    }

    private void Update()
    { 
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
        transform.Translate(movement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key was pressed.");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
        }
    }
}
