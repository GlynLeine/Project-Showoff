using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCaps : MonoBehaviour
{
    public Transform north;
    public Transform south;
    public Transform planet;

    public float northDip;
    public float southDip;

    float northDist;
    float southDist;

    private void Start()
    {
        northDist = Vector3.Distance(planet.position, north.position);
        southDist = Vector3.Distance(planet.position, south.position);
    }

    private void Update()
    {
        Vector3 toNorth = (north.position - planet.position).normalized;
        north.position = planet.position + toNorth * GameManager.lerp(northDist - northDip, northDist, GameManager.iceCaps);

        Vector3 toSouth = (south.position - planet.position).normalized;
        south.position = planet.position + toSouth * GameManager.lerp(southDist - southDip, southDist, GameManager.iceCaps);
    }
}
