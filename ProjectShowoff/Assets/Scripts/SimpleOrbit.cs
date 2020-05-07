using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOrbit : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float range;
    public Vector3 right;

    void Start()
    {
        range = (target.position - transform.position).magnitude;
        right = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (speed * range * Time.deltaTime) / 100 * transform.up;
        Vector3 normal = (transform.position - target.position).normalized;
        transform.position = target.position + (normal * range);
        transform.rotation = Quaternion.LookRotation(-normal, Vector3.Cross(right, normal));
    }
}
