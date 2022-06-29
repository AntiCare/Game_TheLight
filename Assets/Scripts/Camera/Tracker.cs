using System;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    private Transform trackedObject;
    public  Vector2   trackingOffset;
    private Vector3   offset;

    void Start()
    {
        trackedObject = GameObject.FindGameObjectWithTag("Player").transform;
        offset        = (Vector3) trackingOffset;
        offset.z      = transform.position.z - trackedObject.position.z;
    }

    void LateUpdate()
    {
        if (trackedObject == null)
        {
            return;
        }

        float height = Screen.height;
        float width  = Screen.width;

        transform.position = Vector3.MoveTowards(
            transform.position,
            trackedObject.position + offset,
            20f // Might bring bugs in the future
        );
    }
}