using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnowController : MonoBehaviour
{
    public UnityEvent SnowOn;
    public UnityEvent SnowOff;

    public void SnowButton()
    {
        SnowOn.Invoke();
    }
}
