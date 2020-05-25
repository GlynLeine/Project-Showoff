using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCasterTestScript : MonoBehaviour
{
    [SerializeField] private GameObject textScript = null;
    private AnnouncerText atScript;
    private float timer = 0;
    void Start()
    {
        atScript = textScript.GetComponent<AnnouncerText>();
    }

    private void Update()
    {
        timer += 1 * Time.deltaTime;
        if (timer > 5)
        {
            atScript.TextChanger("changing it to a really long paragraph real quick that has like looooooads of words but look it still comes back right on time!");
            timer = 0;
        }
    }
}
