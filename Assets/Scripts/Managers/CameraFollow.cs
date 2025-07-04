using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;

    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate the initial offset.
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Create a position the camera is aiming for based on the offset from the target.
        Vector3 targetCamPos = target.position + offset;
        
        // Smoothly interpolate between the camera's current position and it's target position.
        transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
