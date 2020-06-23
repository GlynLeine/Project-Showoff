using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokable : MonoBehaviour
{
    public bool pokePlay = false;
    public float animCoolDown = 2;
    [FMODUnity.EventRef] 
    private FMOD.Studio.EventInstance pokeEvent;
    private string pokedName = "Robot_poked";

    void OnEnable()
    {
        Debug.Log("instantiated");
        //pokeEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Robot poked");
    }
    IEnumerator reset()
    {
        yield return new WaitForSeconds(animCoolDown + Time.deltaTime);
        pokePlay = false;

    }
    public void poke()
    {
        GameManager.creaturesPoked++;
        //pokeEvent.start();
        //pokeEvent.setParameterByName(pokedName, 1f);
        pokePlay = true;
        StartCoroutine(reset());
    }
   
}
