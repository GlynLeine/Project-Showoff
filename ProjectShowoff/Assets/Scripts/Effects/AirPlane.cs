using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlane : MonoBehaviour
{
    public TrainStation origin;
    public TrainStation destination;
    public float acceleration;

    void Start()
    {
        StartCoroutine(FlightPath());
    }

    IEnumerator FlightPath()
    {
        yield return null;

        Spline takeOff = origin.GetComponent<Spline>();
        Spline landing = this.destination.GetComponent<Spline>();

        float distanceTraveled = 0;
        float travelSpeed = 0;
        float destination = takeOff.length;
        Vector3 prevpos = transform.position;

        Vector3 velocity;
        float takeOffSpeed = 0;

        bool endReached = false;
        while (!endReached)
        {
            Debug.Log(travelSpeed);
            travelSpeed += acceleration * GameManager.deltaTime;
            distanceTraveled += travelSpeed * GameManager.deltaTime;
            endReached = distanceTraveled >= destination;

            prevpos = transform.position;
            float pointAlongSpline = takeOff.length - distanceTraveled;
            transform.position = takeOff.GetWorldPointAtDistance(pointAlongSpline);

            velocity = transform.position - prevpos;
            takeOffSpeed = velocity.magnitude;
            if (takeOffSpeed > 0)
                transform.rotation = Quaternion.LookRotation(velocity / takeOffSpeed, takeOff.GetWorldRotationAtDistance(pointAlongSpline) * Vector3.right);

            yield return null;
        }

        Debug.Log(travelSpeed);
        Vector3 targetPosition = landing.GetWorldPointAtDistance(0);
        float distanceFromTarget = Vector3.Distance(transform.position, targetPosition);
        float speed = 0;
        while (distanceFromTarget > travelSpeed * GameManager.deltaTime)
        {
            prevpos = transform.position;
            transform.position = transform.position + (targetPosition - transform.position).normalized * travelSpeed * GameManager.deltaTime;

            velocity = transform.position - prevpos;
            speed = velocity.magnitude;
            if (speed > 0)
                transform.rotation = Quaternion.LookRotation(velocity / speed, landing.GetWorldRotationAtDistance(distanceTraveled) * Vector3.right);

            yield return null;
        }

        prevpos = transform.position;
        transform.position = targetPosition;

        velocity = transform.position - prevpos;
        speed = velocity.magnitude;
        if (speed > 0)
            transform.rotation = Quaternion.LookRotation(velocity / speed, landing.GetWorldRotationAtDistance(distanceTraveled) * Vector3.right);

        distanceTraveled = 0;
        acceleration = 1f;
        destination = landing.length;

        endReached = false;
        float initialSpeed = travelSpeed;
        while (!endReached && travelSpeed >= 0.01f * initialSpeed)
        {
            travelSpeed = initialSpeed * GameManager.smoothstep(landing.length, 0, distanceTraveled);
            Debug.Log(travelSpeed);

            distanceTraveled += travelSpeed * GameManager.deltaTime;
            endReached = distanceTraveled >= destination;

            prevpos = transform.position;
            transform.position = landing.GetWorldPointAtDistance(distanceTraveled);

            velocity = transform.position - prevpos;
            speed = velocity.magnitude;
            if (speed > 0)
                transform.rotation = Quaternion.LookRotation(velocity / speed, landing.GetWorldRotationAtDistance(distanceTraveled) * Vector3.right);
            Debug.Log("landing");
            yield return null;
        }

        Destroy(gameObject);
    }
}
