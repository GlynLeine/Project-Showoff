using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
{
    public Transform target;
    public bool inverted = false;
    // Update is called once per frame
    void Update()
    {
        if (inverted)
            transform.forward = (target.position - transform.position).normalized;
        else
            transform.forward = (transform.position - target.position).normalized;
    }
}
