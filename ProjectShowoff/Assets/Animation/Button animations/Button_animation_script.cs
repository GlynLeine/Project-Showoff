using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_animation_script : MonoBehaviour
{
    private Animator m_Animator;
    private bool animationPlaying;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    
    public void button_touch()
    {
        animationPlaying = true;
    }
}
