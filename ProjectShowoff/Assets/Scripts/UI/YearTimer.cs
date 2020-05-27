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
            
        }
        if (timer > 60)
        {
            if (timer <= 120)
            {
                
            }

            if (timer > 120)
            {
                
            }
        }
    }
}
