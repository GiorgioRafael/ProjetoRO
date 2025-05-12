using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    public Transform target;

    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [SerializeField]
    private float smoothTime;

    private Vector3 velocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
