using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffect : MonoBehaviour
{
    public GameObject cloudPrefab;

    public int numberOfClouds = 100;
    public float cloudHeight = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numberOfClouds; i++)
        {
            float step = (float)i / numberOfClouds;
            float phi = Mathf.Acos(1 - 2 * step);
            float theta = angleIncrement * i;

            Vector3 offset = new Vector3();
            offset.x = Mathf.Sin(phi) * Mathf.Cos(theta);
            offset.y = Mathf.Sin(phi) * Mathf.Sin(theta);
            offset.z = Mathf.Cos(phi);

            offset *= cloudHeight;

            GameObject cloud = Instantiate(cloudPrefab, transform.position + offset, Quaternion.identity, transform);
            //cloud.isStatic = true;
        }
    }
}
