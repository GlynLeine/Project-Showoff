using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Orbit : MonoBehaviour
{
    static List<Orbit> orbits;
    static float G;

    public Vector3 initialVelocity;
    public float gravitationalConstant = 0.000000000667f;
    public new Rigidbody rigidbody;

    private void OnValidate()
    {
        if (gravitationalConstant != G)
            G = gravitationalConstant;
    }

    static Orbit()
    {
        orbits = new List<Orbit>();
    }

    private void Start()
    {
        orbits.Add(this);
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = initialVelocity;
    }

    private void Update()
    {
        foreach (Orbit orbit in orbits)
            if (orbit != this)
            {
                Vector3 forceNormal = (orbit.transform.position - transform.position);
                Vector3 F = (((G * rigidbody.mass) / forceNormal.sqrMagnitude) * orbit.rigidbody.mass) * forceNormal.normalized;
                rigidbody.velocity += F;
            }
    }
}
