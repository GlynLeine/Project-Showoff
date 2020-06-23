using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarbourManager : MonoBehaviour
{
    public Boat[] boats;

    private void Start()
    {
        foreach(Boat boat in boats)
        {
            boat.ResetPosition();
        }
    }

    void Update()
    {
        if (GameManager.industry >= 10)
        {
            foreach (Boat boat in boats)
                if (!boat.active)
                {
                    Vector3 campos = Camera.main.transform.position;
                    boat.activeModel = Random.Range(0, boat.models.Length);
                    Renderer renderer = boat.models[boat.activeModel];

                    Vector3 toOcean = (campos * -1f).normalized;
                    Vector3 toObject = renderer.bounds.center - campos;
                    float projection = Vector3.Dot(toOcean, toObject);
                    toObject = (toObject - (toOcean * projection)).normalized;

                    float radius = Mathf.Max(new float[] { renderer.bounds.extents.x, renderer.bounds.extents.y, renderer.bounds.extents.z });

                    Vector3 rayCastTarget = renderer.bounds.center + toObject * radius;

                    if (Physics.Raycast(new Ray(campos, (rayCastTarget - campos).normalized), out RaycastHit hit, Vector3.Distance(rayCastTarget, campos), LayerMask.GetMask("Ocean")))
                    {
                        boat.gameObject.SetActive(true);
                        boat.active = true;
                    }
                }
        }
        else
            foreach (Boat boat in boats)
                if (boat.active)
                {
                    Vector3 campos = Camera.main.transform.position;
                    Renderer renderer = boat.models[boat.activeModel];

                    Vector3 toOcean = (campos * -1f).normalized;
                    Vector3 toObject = renderer.bounds.center - campos;
                    float projection = Vector3.Dot(toOcean, toObject);
                    toObject = (toObject - (toOcean * projection)).normalized;

                    float radius = Mathf.Max(new float[] { renderer.bounds.extents.x, renderer.bounds.extents.y, renderer.bounds.extents.z });

                    Vector3 rayCastTarget = renderer.bounds.center + toObject * radius;

                    if (Physics.Raycast(new Ray(campos, (rayCastTarget - campos).normalized), out RaycastHit hit, Vector3.Distance(rayCastTarget, campos), LayerMask.GetMask("Ocean")))
                    {
                        boat.gameObject.SetActive(false);
                        boat.active = false;
                    }
                }

    }
}
