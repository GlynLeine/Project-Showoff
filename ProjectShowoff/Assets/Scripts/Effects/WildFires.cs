using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WildFires : MonoBehaviour
{
    List<WildFire_Get> wildfires;
    bool fire;
    private void Start()
    {
        wildfires = new List<WildFire_Get>(FindObjectsOfType<WildFire_Get>());

        foreach (WildFire_Get wildfire in wildfires)
            wildfire.gameObject.SetActive(false);

        fire = false;
    }

    private void Update()
    {
        if (!fire && GameManager.ozone > 0.7f)
            StartCoroutine(Enable(true));
        else if (fire && GameManager.ozone <= 0.7f)
            StartCoroutine(Enable(false));
    }

    IEnumerator Enable(bool enable)
    {
        fire = enable;
        int burnCount = 0;
        while (burnCount < wildfires.Count)
        {
            for (int i = 0; i < wildfires.Count; i++)
            {
                if (wildfires[i].burning != enable)
                    if (Random.value < 0.01f * GameManager.deltaTime)
                    {
                        if (enable)
                            wildfires[i].Enable();
                        else
                            wildfires[i].Disable();

                        burnCount++;
                    }
            }
            yield return null;
        }
    }
}
