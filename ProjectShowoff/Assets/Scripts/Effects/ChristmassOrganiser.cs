using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmassOrganiser : MonoBehaviour
{
    ChristmassTree[] trees;

    private void Start()
    {
        trees = FindObjectsOfType<ChristmassTree>();

        foreach (ChristmassTree tree in trees)
            tree.gameObject.SetActive(false);

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (GameManager.season < 0.8f)
            yield return null;

        foreach(ChristmassTree tree in trees)
            tree.gameObject.SetActive(true);
    }
}
