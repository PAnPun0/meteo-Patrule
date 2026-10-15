using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.2f; 
    public Vector3 offset = new Vector3(0f, 1.5f, -10f); 

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
    }
}