using UnityEngine;

public class KameraScript : MonoBehaviour
{
    public float mouseSensitivity = 100f;


    public Transform player;
    [SerializeField] private Vector3 offset;

    void Update()
    {
        transform.position = player.position + offset;
    }
}
