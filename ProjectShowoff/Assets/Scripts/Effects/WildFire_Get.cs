using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFire_Get : MonoBehaviour
{
    public new ParticleSystem particleSystem;
    public void Enable()
    {
        particleSystem.Play();
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        particleSystem.Stop();
        StartCoroutine(SetNonActive());
    }

    IEnumerator SetNonActive()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(true);
    }
}
