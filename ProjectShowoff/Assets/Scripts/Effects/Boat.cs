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
        distance += speed * direction * Time.deltaTime;
        Vector3 position = boatSpline.GetWorldPointAtDistance(distance);
        position = position.normalized * (1f + GameManager.waterLevel * 0.07f);
        transform.position = position;
    }
}
