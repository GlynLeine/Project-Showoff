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

    void Start()
    {
        StartCoroutine(Wander());
        buildingSystem = FindObjectOfType<BuildingSystem>();
    }

    IEnumerator Wander()
    {
        while (true)
        {
            bool travel = Random.Range(0f, 1f) >= travelChance;
            BuildingLocation targetLocation;
            if (travel)
                targetLocation = buildingSystem.GetValidTravelLocation();
            else
                targetLocation = location;

            Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, wanderRange);
            Vector3 position = targetLocation.transform.position;
            position += targetLocation.transform.forward * offset.x + targetLocation.transform.right * offset.y;
            walkTarget = new WalkTarget(position, targetLocation);
            yield return new WaitForSeconds(Random.Range(minWanderTime, maxWanderTime));
        }
    }

    private void Update()
    {
        if (walkTarget != null)
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
                if(walkTarget.path == null)
                    walkTarget.path = buildingSystem.GetPath(location, walkTarget.targetLocation);

                if (walkTarget.path == null || walkTarget.path.Count == 0)
                {
                    walkTarget = null;
                    return;
                }

                BuildingLocation nextLocation = walkTarget.path.Dequeue();
                if(nextLocation == location)
                    return;

                // move to next location

            }
        }
    }


}
