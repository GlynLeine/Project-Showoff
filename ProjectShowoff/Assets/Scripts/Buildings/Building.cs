using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [HideInInspector]
    public BuildingType buildingType;

    public float environmentEffect;
    public float pollutionEffect;
    public float industryEffect;
    public float effectPeriod;

    private float timeBuffer = 0;

    private void Start()
    {
        Collider collider = GetComponentInChildren<Collider>();
        if (collider == null)
            transform.GetChild(0).gameObject.AddComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        //timeBuffer += Time.deltaTime;
        //if (timeBuffer >= effectPeriod)
        //{
        //    timeBuffer -= effectPeriod;
        //    GameManager.AddState(environmentEffect, pollutionEffect, industryEffect);
        //}
        if (effectPeriod > 0f)
        {
            float scale = Time.deltaTime / effectPeriod;
            GameManager.AddState(environmentEffect * scale, pollutionEffect * scale, industryEffect * scale);
        }        
    }
}


