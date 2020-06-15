using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCasterTestScript : MonoBehaviour
{
    [FMODUnity.EventRef] 
    public float blablaVoiceLines;
    [SerializeField] private GameObject textScript = null;
    private AnnouncerText atScript;
    private float timer = 0;
    void Start()
    {
        atScript = textScript.GetComponent<AnnouncerText>();
    }

    private void Update()
    {
        timer += GameManager.deltaTime;
        if (timer > 5)
        {
            atScript.TextChanger("World News!");
            timer = 0;
        }
    }
}
