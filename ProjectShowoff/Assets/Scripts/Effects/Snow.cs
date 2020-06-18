using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Snow : MonoBehaviour
{
    public GameObject snowEffectPrefab;

    VisualEffect snowEffect;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (GameManager.season < 0.7f)
            yield return null;

        snowEffect = Instantiate(snowEffectPrefab, transform).GetComponent<VisualEffect>();
        snowEffect.SetFloat("SnowRate", 0f);

        while (GameManager.season < 1f)
        {
            float snowRate = GameManager.smoothstep(0.7f, 1f, GameManager.season);
            snowEffect.SetFloat("SnowRate", GameManager.lerp(snowRate * snowRate, 0, GameManager.climate));
            yield return null;
        }

        snowEffect.SetFloat("SnowRate", 1f);
    }
}
