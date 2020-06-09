using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokable : MonoBehaviour
{
    public bool pokePlay = false;
    public float animCoolDown = 2;

    private void Update()
    {
        //Debug.Log(pokePlay);
    }
    IEnumerator reset()
    {
        yield return new WaitForSeconds(animCoolDown + Time.deltaTime);
        pokePlay = false;
    }
    public void poke()
    {
        GameManager.creaturesPoked++;
        //fmod code needed
        pokePlay = true;
        StartCoroutine(reset());
    }
   
}
