using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_animation_script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void button_touch()
    {
        GetComponent<Animator>().Play("");
    }
}
