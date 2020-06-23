using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Spline boatSpline;

    float distance;
    public float speed;
    float direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (distance > boatSpline.length)
        {
            direction = -1f;
        }
        else if(distance < 0)
        {
            direction = 1f;
        }
        distance += speed * direction * Time.deltaTime;
        transform.position = boatSpline.GetWorldPointAtDistance(distance);
    }
}
