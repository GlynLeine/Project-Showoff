using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmassOrganiser : MonoBehaviour
{
    ChristmassTree[] trees;

    public GameObject santaPrefab;
    public float santaAltitude;
    public float santaSpeed;

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

        Santa santa = Instantiate(santaPrefab).GetComponent<Santa>();
        santa.transform.position = Camera.main.transform.position.normalized * -santaAltitude;
        santa.travelSpeed = santaSpeed;

        if (GameManager.industry >= 20f)
            foreach (ChristmassTree tree in trees)
                tree.gameObject.SetActive(true);
    }
}
