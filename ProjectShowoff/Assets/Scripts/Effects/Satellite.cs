using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{
    Vector3 target;
    Vector3 origin;
    Vector3 prevPos;
    float traveled;
    public float travelSpeed;
    public Technology technology;

    public GameObject[] models;

    new Renderer renderer;

    void Start()
    {
        foreach(GameObject model in models)
            model.SetActive(false);

        models[Random.Range(0, models.Length)].SetActive(true);

        target = transform.position * -1f;
        origin = transform.position;
        renderer = GetComponentInChildren<Renderer>();

        Vector3 forward = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Vector3 up = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Vector3 right = Vector3.Cross(up, forward).normalized;
        forward = Vector3.Cross(right, up).normalized;

        transform.rotation = Quaternion.LookRotation(forward, up);
    }

    void Update()
    {
        traveled += travelSpeed * Time.deltaTime;
        prevPos = transform.position;
        transform.position = Vector3.SlerpUnclamped(origin, target, traveled);

        if (technology.level < 2)
        {
            Vector3 campos = Camera.main.transform.position;

            Vector3 toOcean = (campos * -1f).normalized;
            Vector3 toObject = renderer.bounds.center - campos;
            float projection = Vector3.Dot(toOcean, toObject);
            toObject = (toObject - (toOcean * projection)).normalized;

            float radius = Mathf.Max(new float[] { renderer.bounds.extents.x, renderer.bounds.extents.y, renderer.bounds.extents.z });

            Vector3 rayCastTarget = renderer.bounds.center + toObject * radius;

            if (Physics.Raycast(new Ray(campos, (rayCastTarget - campos).normalized), out RaycastHit hit, Vector3.Distance(rayCastTarget, campos), LayerMask.GetMask("Ocean")))
                Destroy(gameObject);
        }
    }
}
