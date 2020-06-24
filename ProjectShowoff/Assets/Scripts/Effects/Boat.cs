using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Spline boatSpline;
    public float speed;
    public SphereCollider oceanCollider;

    public Renderer[] models;
    [HideInInspector]
    public int activeModel;
    [HideInInspector]
    public bool active;

    Vector3 target;
    Vector3 nextTarget;
    Vector3 prevTarget;

    public void ResetPosition()
    {
        transform.position = boatSpline.GetWorldPointAtDistance(0);
        transform.up = transform.position.normalized;
    }

    void Start()
    {
        models[activeModel].gameObject.SetActive(true);
        StartCoroutine(Travel());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(nextTarget, 0.01f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.01f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(prevTarget, 0.01f);
    }

    IEnumerator Travel()
    {
        float distance = 0;
        int segmentIndex = 0;
        while (true)
        {
            VertexPath vertexPath = boatSpline.VertexPath;
            distance = vertexPath.GetDistance(segmentIndex);
            float nextDistance = vertexPath.GetDistance(segmentIndex + 1);
            if (segmentIndex + 1 == vertexPath.VertexCount)
                nextDistance = vertexPath.GetDistance(segmentIndex + 3);

            float travelDistance = distance;

            prevTarget = target;
            target = oceanCollider.bounds.center + boatSpline.GetWorldPointAtDistance(distance).normalized * oceanCollider.radius * (1f + GameManager.waterLevel * 0.07f);
            nextTarget = oceanCollider.bounds.center + boatSpline.GetWorldPointAtDistance(nextDistance).normalized * oceanCollider.radius * (1f + GameManager.waterLevel * 0.07f);

            Vector3 currentforward = (target - prevTarget).normalized;
            Vector3 Nextforward = (nextTarget - target).normalized;

            while (Vector3.Distance(target, transform.position) > speed * Time.deltaTime)
            {
                travelDistance += speed * Time.deltaTime;
                transform.position += (target - transform.position).normalized * speed * Time.deltaTime;

                Vector3 forward = Vector3.Slerp(currentforward, Nextforward, GameManager.smoothstep(distance, nextDistance, travelDistance));
                Vector3 up = (transform.position - oceanCollider.bounds.center).normalized;
                transform.rotation = Quaternion.LookRotation(forward, up);
                yield return null;
            }

            segmentIndex = vertexPath.LoopIndex(segmentIndex + 1);
            yield return null;
        }
    }
}
