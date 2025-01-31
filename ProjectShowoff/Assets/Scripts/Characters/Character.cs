﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public class WalkTarget
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

    private Vector3 prevpos;
    public float velocity;
    public BuildingLocation location;
    public float wanderRange;
    public float walkSpeed;
    public float actualWalkSpeed;
    public float travelChance;
    public Transform planet;
    public float minWanderTime;
    public float maxWanderTime;

    BuildingSystem buildingSystem;
    public WalkTarget walkTarget;
    bool travelling;
    private Animator animator;
    private Pokable pokable;

    void Start()
    {
        prevpos = transform.position;
        animator = GetComponentInChildren<Animator>();
        pokable = GetComponent<Pokable>();
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

            float walkDistance = actualWalkSpeed * GameManager.deltaTime;
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
                walkedDistance += actualWalkSpeed * GameManager.deltaTime * walkDirection;
                locationReached = walkedDistance * walkDirection >= destination;

                prevpos = transform.position;
                transform.position = road.spline.GetWorldPointAtDistance(walkedDistance);

                Vector3 vel = transform.position - prevpos;
                velocity = vel.magnitude;
                if (velocity > 0)
                    transform.rotation = Quaternion.LookRotation(vel / velocity, road.spline.GetWorldRotationAtDistance(walkedDistance) * Vector3.up);

                yield return null;
            }

            location = nextLocation;
            yield return null;
        }

        travelling = false;
    }

    private void Update()
    {
        if (pokable.pokePlay)
        {
            actualWalkSpeed = 0f;
            prevpos = transform.position;
        }
        else
            actualWalkSpeed = walkSpeed;

        Vector3 vel = transform.position - prevpos;
        velocity = vel.magnitude;
        if (velocity > 0)
            transform.rotation = Quaternion.LookRotation(vel / velocity, location.transform.up);

        prevpos = transform.position;

        animator.SetFloat("velocity", velocity);
        animator.SetBool("pokePlay", pokable.pokePlay);

        if (walkTarget != null && !travelling)
        {
            if (walkTarget.targetLocation == location)
            {
                Vector3 difference = walkTarget.position - transform.position;
                float distance = difference.magnitude;
                Vector3 direction = difference.normalized;

                float walkDistance = actualWalkSpeed * GameManager.deltaTime;
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
                    AbortPath();
                    return;
                }
                StartCoroutine(TravelToNewLocation());
            }
        }
    }


}
