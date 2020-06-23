﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFire_Get : MonoBehaviour
{
    public new ParticleSystem particleSystem;

    public bool burning = false;
    public void Enable()
    {
        burning = true;
        particleSystem.Play();
        gameObject.SetActive(true);
        StartCoroutine(BurnTree());
    }

    IEnumerator BurnTree()
    {
        yield return new WaitForSeconds(15);
        transform.parent.gameObject.SetActive(false);
    }

    public void Disable()
    {
        burning = false;
        particleSystem.Stop();
        StartCoroutine(SetNonActive());
    }

    IEnumerator SetNonActive()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(true);
    }
}
