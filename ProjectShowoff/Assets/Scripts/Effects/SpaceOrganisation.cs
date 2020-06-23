using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpaceOrganisation : MonoBehaviour
{
    bool planning = false;

    public GameObject[] spaceshipPrefabs;
    public float minExpeditionInterval;
    public float maxExpeditionInterval;
    public float acceleration;

    public GameObject satellitePrefab;
    public float satelliteOrbitSpeed;
    public float satelliteOrbitAltitude;

    public void StartPlanningExpeditions(Technology technology)
    {
        if (!planning)
            StartCoroutine(PlanExpeditions(technology));
    }

    IEnumerator PlanExpeditions(Technology technology)
    {
        Debug.Log("Started planning expeditions");
        planning = true;

        while (technology.level > 1)
        {
            if (technology.trainStations.Count > 0)
            {
                TrainStation origin = technology.trainStations[Random.Range(0, technology.trainStations.Count)];

                Spaceship spaceship = Instantiate(spaceshipPrefabs[Random.Range(0, spaceshipPrefabs.Length)]).GetComponent<Spaceship>();
                spaceship.gameObject.name = "Spaceship " + Spaceship.spaceships;
                spaceship.acceleration = acceleration;

                spaceship.transform.position = origin.transform.position;
                spaceship.transform.rotation = origin.transform.rotation;

                if (Spaceship.spaceships % 3 == 0)
                    StartCoroutine(LaunchSatellite(spaceship, technology));

                Spaceship.spaceships++;
                Debug.Log("spawned spaceship");
            }

            yield return new WaitForSeconds(Random.Range(minExpeditionInterval, maxExpeditionInterval));
        }

        planning = false;
        Debug.Log("Stopped planning expeditions");
    }

    IEnumerator LaunchSatellite(Spaceship spaceship, Technology technology)
    {
        spaceship.carryingSatellite = true;
        while (spaceship.transform.position.magnitude < satelliteOrbitAltitude)
            yield return null;

        spaceship.carryingSatellite = false;

        Satellite satellite = Instantiate(satellitePrefab).GetComponent<Satellite>();
        Vector3 dir = Camera.main.transform.position * -1f;
        dir = (dir + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * dir.magnitude * 0.75f)).normalized;
        satellite.transform.position = dir * satelliteOrbitAltitude;
        satellite.travelSpeed = satelliteOrbitSpeed;
        satellite.technology = technology;

        satelliteOrbitAltitude += 0.06f;
    }
}
