using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Poke : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputRedirect.pressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputRedirect.inputPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Pokable pokable = hit.collider.gameObject.GetComponent<Pokable>();
                if (pokable != null)
                {
                    pokable.poke();
                    Debug.Log("Poked");
                }
            }
        }
    }

 
}
