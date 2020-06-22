using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingVariation : MonoBehaviour
{
    public GameObject[] models;
    // Start is called before the first frame update
    void Awake()
    {      
        foreach (GameObject child in models)
        {
            child.SetActive(false);
        }

        int randomModel = Random.Range(0, models.Length);

        models[randomModel].gameObject.SetActive(true);
        
    }
}
