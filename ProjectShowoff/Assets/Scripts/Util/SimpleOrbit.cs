using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOrbit : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float range;
    public Vector3 right;
    public Vector3 forward;

    void Start()
    {
        range = (target.position - transform.position).magnitude;

        Vector3 up = (transform.position - target.position).normalized;
        right = transform.right;
        forward = Vector3.Cross(right, up).normalized;
        right = Vector3.Cross(forward, up).normalized;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 up = (transform.position - target.position).normalized;
        forward = Vector3.Cross(right, up).normalized;
        transform.position += (speed * range * GameManager.deltaTime) / 100 * forward;

        Vector3 normal = (transform.position - target.position).normalized;
        transform.position = target.position + (normal * range);
    }
}
