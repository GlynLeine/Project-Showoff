using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YearTimer : MonoBehaviour
{
    private float timer = 0;

    void Update()
    {
        timer += 1 * Time.deltaTime;
        if (timer <= 60)
        {
            //spring 1
        }
        else if (timer > 60 && timer <= 120)
        {
            //summer
        }
        else if (timer > 120 && timer <= 180)
        {
            //fall
        }
        else if (timer > 180 && timer <= 240)
        {
            //winter
        }
        else if (timer > 240 && timer <= 300)
        {
            //spring 2
        }
    }
}
