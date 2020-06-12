using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poke : MonoBehaviour
{
    void Update()
    {
        if (InputRedirect.tapped)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputRedirect.inputPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Pokable pokable = hit.collider.gameObject.GetComponent<Pokable>();
                pokable?.poke();
            }
        }
    }
}
