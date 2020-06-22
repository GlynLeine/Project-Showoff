using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlane : MonoBehaviour
{
    public static int airplanes = 0;
    public TrainStation origin;
    public TrainStation destination;
    public Spline flightPlan;
    public float acceleration;
    public float apexHeight;
    void Start()
    {
        StartCoroutine(FlightPath());
    }

    IEnumerator FlightPath()
    {
        airplanes++;
        yield return null;

        Spline takeOff = origin.GetComponent<Spline>();
        Spline landing = this.destination.GetComponent<Spline>();

        Vector3 endTakeOff = takeOff.GetWorldPoint(0);
        Vector3 startLanding = landing.GetWorldPoint(0);
        Vector3 flightApex = (endTakeOff + startLanding) / 2f;
        Vector3 flightUp = flightApex.normalized;
        Vector3 startTangent = (endTakeOff - takeOff.GetWorldPoint(1)) * 2f;
        Vector3 endTangent = (startLanding - landing.GetWorldPoint(1)) * 2f;

        apexHeight = GameManager.lerp(1f, apexHeight, Mathf.Clamp01(Vector3.Distance(destination.transform.position, origin.transform.position)));

        flightApex = flightUp * apexHeight + ((startTangent + endTangent) / 2f);

        flightPlan.MovePoint(0, endTakeOff);
        flightPlan.MovePoint(flightPlan.PointCount - 1, startLanding);
        flightPlan.SplitSegment(flightApex, 0);
        flightPlan.AutoTangents = true;
        flightPlan.AutoTangents = false;

        flightPlan.MovePoint(1, startTangent + endTakeOff);
        flightPlan.MovePoint(flightPlan.PointCount - 2, endTangent + startLanding);

        flightPlan.SetNormal(0, -takeOff.GetNormal(0));
        flightPlan.SetNormal(1, flightUp);
        flightPlan.SetNormal(2, -landing.GetNormal(0));

        flightPlan.resolution = 20;
        flightPlan.UpdateVertexPath();
        flightPlan.UpdateVertexPath();
        flightPlan.UpdateVertexPath();
        flightPlan.UpdateVertexPath();

        float distanceTraveled = 0;
        float travelSpeed = 0;
        float destinationDistance = takeOff.length;
        Vector3 prevpos = transform.position;

        Vector3 velocity = Vector3.zero;
        float takeOffSpeed = 0;

        bool endReached = false;
        while (!endReached)
        {
            travelSpeed += acceleration * GameManager.deltaTime;
            distanceTraveled += travelSpeed * GameManager.deltaTime;
            endReached = distanceTraveled >= destinationDistance;

            prevpos = transform.position;
            float pointAlongSpline = takeOff.length - distanceTraveled;
            transform.position = takeOff.GetWorldPointAtDistance(pointAlongSpline);

            velocity = transform.position - prevpos;
            takeOffSpeed = velocity.magnitude;
            if (takeOffSpeed > 0)
                transform.rotation = Quaternion.LookRotation(velocity / takeOffSpeed, takeOff.GetWorldRotationAtDistance(pointAlongSpline) * Vector3.right);

            yield return null;
        }

        distanceTraveled = 0;
        acceleration = 1f;
        destinationDistance = flightPlan.length;
        float speed;

        endReached = false;
        while (!endReached)
        {
            distanceTraveled += travelSpeed * GameManager.deltaTime;
            endReached = distanceTraveled >= destinationDistance;

            prevpos = transform.position;
            transform.position = flightPlan.GetWorldPointAtDistance(distanceTraveled);

            velocity = transform.position - prevpos;
            speed = velocity.magnitude;
            if (speed > 0)
                transform.rotation = Quaternion.LookRotation(velocity / speed, flightPlan.GetWorldRotationAtDistance(distanceTraveled) * Vector3.right);
            yield return null;
        }

        distanceTraveled = 0;
        acceleration = 1f;
        destinationDistance = landing.length;

        endReached = false;
        float initialSpeed = travelSpeed;
        while (!endReached && travelSpeed >= 0.01f * initialSpeed)
        {
            travelSpeed = initialSpeed * GameManager.smoothstep(landing.length, 0, distanceTraveled);

            distanceTraveled += travelSpeed * GameManager.deltaTime;
            endReached = distanceTraveled >= destinationDistance;

            prevpos = transform.position;
            transform.position = landing.GetWorldPointAtDistance(distanceTraveled);

            velocity = transform.position - prevpos;
            speed = velocity.magnitude;
            if (speed > 0)
                transform.rotation = Quaternion.LookRotation(velocity / speed, landing.GetWorldRotationAtDistance(distanceTraveled) * Vector3.right);
            yield return null;
        }

        origin.arrival = false;
        destination.arrival = false;

        Destroy(gameObject);
        Destroy(flightPlan.gameObject);
    }
}
