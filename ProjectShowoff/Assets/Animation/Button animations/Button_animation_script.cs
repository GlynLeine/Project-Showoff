using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_animation_script : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    public void button_touch()
    {
        animator?.SetTrigger("play");
    }
}
