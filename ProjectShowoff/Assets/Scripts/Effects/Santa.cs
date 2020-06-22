using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Santa : MonoBehaviour
{
    Vector3 target;
    Vector3 origin;
    Vector3 prevPos;
    float traveled;
    public float travelSpeed;

    new Renderer renderer;

    void Start()
    {
        target = transform.position * -1f;
        origin = transform.position;
        renderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        traveled += travelSpeed * Time.deltaTime;
        prevPos = transform.position;
        transform.position = Vector3.SlerpUnclamped(origin, target, traveled);

        Vector3 forward = (transform.position - prevPos).normalized;
        Vector3 up = transform.position.normalized;
        Vector3 right = Vector3.Cross(up, forward).normalized;
        forward = Vector3.Cross(right, up).normalized;

        transform.rotation = Quaternion.LookRotation(forward, up);

        if (traveled > 1.5f)
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
