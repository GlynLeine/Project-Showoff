using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FlightPlanner : MonoBehaviour
{
    bool planning = false;

    public GameObject[] airPlanePrefabs;
    public float minFlightInterval;
    public float maxFlightInterval;
    public float planeAcceleration;
    public float apexHeight;

    public void StartPlanningFlights(Technology technology)
    {
        if (!planning)
            StartCoroutine(PlanFlights(technology));
    }

    IEnumerator PlanFlights(Technology technology)
    {
        Debug.Log("Started planning flights");
        planning = true;

        while (technology.level == 1)
        {
            List<TrainStation> options = technology.trainStations.ToList();
            for (int i = 0; i < options.Count; i++)
                if (options[i].arrival)
                {
                    options.RemoveAt(i);
                    i--;
                }

            if (options.Count >= 2)
            {
                TrainStation origin = options[Random.Range(0, options.Count)];
                origin.arrival = true;

                options.Remove(origin);

                TrainStation destination = options[Random.Range(0, options.Count)];
                destination.arrival = true;

                AirPlane airPlane = Instantiate(airPlanePrefabs[Random.Range(0, airPlanePrefabs.Length)]).GetComponent<AirPlane>();
                airPlane.gameObject.name = "Airplane " + AirPlane.airplanes;
                airPlane.origin = origin;
                airPlane.destination = destination;
                airPlane.acceleration = planeAcceleration;
                Spline flightPlan = new GameObject("Flight Plan " + AirPlane.airplanes).AddComponent<Spline>();
                airPlane.flightPlan = flightPlan;
                airPlane.apexHeight = apexHeight;
                Debug.Log("spawned plane");
            }

            yield return new WaitForSeconds(Random.Range(minFlightInterval, maxFlightInterval));
        }

        planning = false;
        Debug.Log("Stopped planning flights");
    }
}
