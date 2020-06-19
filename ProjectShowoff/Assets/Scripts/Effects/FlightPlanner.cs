using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FlightPlanner : MonoBehaviour
{
    bool planning = false;

    public GameObject airPlanePrefab;
    public float minFlightInterval;
    public float maxFlightInterval;
    public float planeAcceleration;

    public void StartPlanningFlights(Technology technology)
    {
        if (!planning)
            StartCoroutine(PlanFlights(technology));
    }

    IEnumerator PlanFlights(Technology technology)
    {
        while (technology.level == 1)
        {
            while (technology.trainStations.Count < 2)
                yield return new WaitForSeconds(Random.Range(minFlightInterval, maxFlightInterval));

            List<TrainStation> options = technology.trainStations.ToList();
            TrainStation origin = options[Random.Range(0, options.Count)];
            options.Remove(origin);
            TrainStation destination = options[Random.Range(0, options.Count)];

            AirPlane airPlane = Instantiate(airPlanePrefab).GetComponent<AirPlane>();
            airPlane.origin = origin;
            airPlane.destination = destination;
            airPlane.acceleration = planeAcceleration;
            Debug.Log("spawned plane");

            yield return new WaitForSeconds(Random.Range(minFlightInterval, maxFlightInterval));
        }
    }
}
