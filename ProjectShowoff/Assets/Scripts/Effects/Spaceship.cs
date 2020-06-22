using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    public static int spaceships = 0;

    new Renderer renderer;

    public float acceleration;

    Vector3 velocity;

    private void Start()
    {
        spaceships++;
        renderer = GetComponentInChildren<Renderer>();
        velocity = new Vector3();
    }

    void Update()
    {
        velocity += transform.up * Time.deltaTime * acceleration;

        transform.position += velocity * Time.deltaTime;

        Bounds bounds = renderer.bounds;
        Vector3 screenCenter = Camera.main.WorldToViewportPoint(bounds.center);
        Vector3 screenMin = Camera.main.WorldToViewportPoint(bounds.min);
        float radius = Vector3.Distance(screenCenter, screenMin);

        if (screenCenter.x > -radius && screenCenter.y > -radius && screenCenter.z > -radius &&
            screenCenter.x < 1 + radius && screenCenter.y < 1 + radius && screenCenter.z < 1 + radius)
            return;

        Destroy(gameObject);
        spaceships--;
    }
}
