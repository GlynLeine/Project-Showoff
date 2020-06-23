using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PauseUnpauseButton : MonoBehaviour
{
    public void PauseClick()
    {
        GameManager.paused = true;
    }

    public void UnPauseClick()
    {
        GameManager.paused = false;
    }
}
