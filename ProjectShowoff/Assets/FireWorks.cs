﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireWorks : MonoBehaviour
{
    public GameObject fireWorksEffectPrefab;

    List<VisualEffect> fireWorks = new List<VisualEffect>();

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (GameManager.season < 1f)
            yield return null;

        if (GameManager.industry > 200)
        {
            foreach (Building building in FindObjectsOfType<Building>())
            {
                fireWorks.Add(Instantiate(fireWorksEffectPrefab, building.transform).GetComponent<VisualEffect>());
            }

            StartCoroutine(DestroyEffect());
        }
    }

    IEnumerator DestroyEffect()
    {
        yield return new WaitForSeconds(10f);

        foreach (VisualEffect fireWork in fireWorks)
            fireWork.Stop();

        yield return new WaitForSeconds(2.5f);

        foreach (VisualEffect fireWork in fireWorks)
            Destroy(fireWork.gameObject);

        fireWorks = null;
    }
}
