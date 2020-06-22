using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFires : MonoBehaviour
{
    List <WildFire_Get> wildfires;

    private void Start()
    {
        wildfires = new List <WildFire_Get> (FindObjectsOfType<WildFire_Get>());

        foreach (WildFire_Get wildfire in wildfires)
            wildfire.gameObject.SetActive(false);

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (GameManager.ozone < 0.5)
            yield return null;

        while (wildfires.Count > 0)
        {
            for (int i = 0; i < wildfires.Count; i++)
            {
                if (Random.value < 0.1f * GameManager.deltaTime)
                {                    
                    wildfires[i].Enable();
                    wildfires.RemoveAt(i);
                    i--;
                }
            }
            yield return null; 
        }
    }
}
