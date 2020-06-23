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

    // Update is called once per frame
    IEnumerator Travel()
    {
        float distance = 0;
        Vector3 prevPos;
        while (true)
        {
            distance += speed * Time.deltaTime;
            Vector3 target = boatSpline.GetWorldPointAtDistance(distance);
            target = oceanCollider.bounds.center + target.normalized * oceanCollider.radius * (1f + GameManager.waterLevel * 0.07f);

            while (Vector3.Distance(target, transform.position) > speed * Time.deltaTime)
            {
                prevPos = transform.position;
                transform.position += (target - transform.position).normalized * speed * Time.deltaTime;

                Vector3 forward = (transform.position - prevPos).normalized;
                Vector3 up = transform.position.normalized;
                transform.rotation = Quaternion.LookRotation(forward, up);

                yield return null;
            }
            yield return null;
        }
    }
}
