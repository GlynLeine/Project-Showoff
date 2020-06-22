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

    public void StartPlanningExpeditions(Technology technology)
    {
        if (!planning)
            StartCoroutine(PlanFlights(technology));
    }

    IEnumerator PlanFlights(Technology technology)
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

                Debug.Log("spawned spaceship");
            }

            yield return new WaitForSeconds(Random.Range(maxExpeditionInterval, maxExpeditionInterval));
        }

        planning = false;
        Debug.Log("Stopped planning expeditions");
    }
}
