using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokable : MonoBehaviour
{
    public bool pokePlay = false;
    public int animCoolDown = 2;

    private void Update()
    {
        //Debug.Log(pokePlay);
    }
    IEnumerator reset()
    {
        yield return new WaitForSeconds(animCoolDown);
        pokePlay = false;

    }
    public void poke()
    {
        //fmod code needed
        pokePlay = true;
        StartCoroutine(reset());
    }
   
}
