using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public float maxZoom;
    public Transform target;
    public float defaultRange;
    public float range;

    private bool ortho;
    private new Camera camera;

    private void OnValidate()
    {
        if (maxZoom < 0.1f)
            maxZoom = 0.1f;
    }

    private void Start()
    {
        defaultRange = (transform.position - target.position).magnitude;
        range = defaultRange;
        camera = GetComponent<Camera>();
        ortho = camera.orthographic;
    }

    // Update is called once per frame
    void Update()
    {
        range = defaultRange - InputRedirect.zoom;
        if (range < maxZoom)
            range = maxZoom;

        if (ortho)
        {
            camera.orthographicSize = range;
        }
        else
        {
            Vector3 dir = (transform.position - target.position).normalized;
            transform.position = target.position + dir * range;
        }
    }
}
