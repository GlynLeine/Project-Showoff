using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffect : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject altCloudPrefab;
    public Material cloudMaterial;

    public int resolution = 100;
    public float cloudHeight = 1.5f;

    public void Start()
    {
        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < resolution; i++)
        {
            float step = (float)i / resolution;
            float phi = Mathf.Acos(1 - 2 * step);
            float theta = angleIncrement * i;

            Vector3 offset = new Vector3();
            offset.x = Mathf.Sin(phi) * Mathf.Cos(theta);
            offset.y = Mathf.Sin(phi) * Mathf.Sin(theta);
            offset.z = Mathf.Cos(phi);

            offset *= cloudHeight;

            bool alt = Random.Range(0f, 1f) > 0.5f;
            GameObject cloud = Instantiate(alt ? altCloudPrefab : cloudPrefab, transform.position + offset, Quaternion.identity, transform);
            cloud.transform.up = offset.normalized;
        }
    }

    private void Update()
    {
        cloudMaterial.SetFloat("_ScaledTime", Time.time);
    }
}
