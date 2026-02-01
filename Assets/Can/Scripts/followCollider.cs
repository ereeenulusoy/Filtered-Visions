using UnityEngine;

public class followCollider : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Target transform is not assigned.", this);
        }
    }
    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position +offset;
            transform.rotation = target.rotation;
        }
    }
}
