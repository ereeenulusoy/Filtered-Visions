using UnityEngine;

public class KameraScript : MonoBehaviour
{
    public Transform target;

    [Header("Camera Settings")]
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 3f;
    public float smoothSpeed = 10f;

    [Header("Clamp")]
    public float minY = -30f;
    public float maxY = 60f;

    private float currentX;
    private float currentY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (!target) return;

        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);

        Vector3 desiredPosition =
            target.position
            - (rotation * Vector3.forward * distance)
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * height);
    }
}