using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    class WalkTarget
    {
        public WalkTarget(Vector3 position, BuildingLocation location)
        {
            this.position = position;
            targetLocation = location;
        }

        public WalkTarget() { }

        public Vector3 position;
        public BuildingLocation targetLocation;
        public Queue<BuildingLocation> path = null;
    }

    public BuildingLocation location;
    public float wanderRange;
    public float walkSpeed;
    public float travelChance;
    public Transform planet;
    public float minWanderTime;
    public float maxWanderTime;

    BuildingSystem buildingSystem;
    WalkTarget walkTarget;
    bool travelling;

    void Start()
    {
        buildingSystem = FindObjectOfType<BuildingSystem>();
        StartCoroutine(Wander());
    }

    public void AbortPath()
    {
        if (walkTarget == null)
            walkTarget = new WalkTarget();

        if (buildingSystem.IsValidTravelLocation(location))
        {
            walkTarget.targetLocation = location;
        }
        else
        {
            walkTarget.targetLocation = buildingSystem.GetValidTravelLocation();
            location = walkTarget.targetLocation;
        }
        walkTarget.position = transform.position;
        walkTarget.path = new Queue<BuildingLocation>();
    }

    IEnumerator Wander()
    {
        while (true)
        {
            if (!travelling)
            {
                bool travel = Random.Range(0f, 1f) < travelChance;
                BuildingLocation targetLocation;
                if (travel)
                {
                    targetLocation = buildingSystem.GetValidTravelLocation(location);
                    if (targetLocation == null)
                    {
                        walkTarget = null;
                        Debug.Log("invalid target.");
                        continue;
                    }
                }
                else
                    targetLocation = location;

                Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, wanderRange);
                Vector3 position = targetLocation.transform.position;
                position += targetLocation.transform.forward * offset.x + targetLocation.transform.right * offset.y;
                walkTarget = new WalkTarget(position, targetLocation);
            }
            yield return new WaitForSeconds(Random.Range(minWanderTime, maxWanderTime));
        }
    }

    IEnumerator TravelToNewLocation()
    {
        travelling = true;
        bool locationReached = false;
        while (!locationReached)
        {
            Vector3 difference = location.transform.position - transform.position;
            float distance = difference.magnitude;
            Vector3 direction = difference.normalized;

            float walkDistance = walkSpeed * Time.deltaTime;
            if (walkDistance > distance)
            {
                walkDistance = distance;
                locationReached = true;
            }

            transform.position += direction * walkDistance;
            yield return null;
        }

        while (walkTarget.path.Count != 0)
        {
            BuildingLocation nextLocation = walkTarget.path.Dequeue();
            if (nextLocation == location)
                continue;

            Road road = location.roads[nextLocation];
            float walkedDistance = 0;
            float walkDirection = 1;
            float destination = road.spline.length;

            if (road.start != location)
            {
                walkedDistance = road.spline.length;
                walkDirection = -1;
                destination = 0;
            }

            locationReached = false;
            while (!locationReached)
            {
                walkedDistance += walkSpeed * Time.deltaTime * walkDirection;
                locationReached = walkedDistance * walkDirection >= destination;
                transform.position = road.spline.GetWorldPointAtDistance(walkedDistance);
                transform.rotation = road.spline.GetWorldRotationAtDistance(walkedDistance);
                yield return null;
            }

            location = nextLocation;
            yield return null;
        }

        travelling = false;
    }

    private void Update()
    {
        if (walkTarget != null && !travelling)
        {
            if (walkTarget.targetLocation == location)
            {
                Vector3 difference = walkTarget.position - transform.position;
                float distance = difference.magnitude;
                Vector3 direction = difference.normalized;

                float walkDistance = walkSpeed * Time.deltaTime;
                if (walkDistance > distance)
                {
                    walkDistance = distance;
                    walkTarget = null;
                }

                transform.position += direction * walkDistance;
            }
            else
            {
                if (walkTarget.path == null)
                    walkTarget.path = buildingSystem.GetPath(location, walkTarget.targetLocation);

                if (walkTarget.path == null || walkTarget.path.Count == 0)
                {
                    walkTarget = null;
                    return;
                }
                StartCoroutine(TravelToNewLocation());
            }
        }
    }


}
