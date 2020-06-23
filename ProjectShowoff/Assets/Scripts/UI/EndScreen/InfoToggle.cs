using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoToggle : MonoBehaviour
{
    private bool switchBool = true;
    public void InfoActiveToggle()
    {
        gameObject.SetActive(switchBool);
        switchBool = !switchBool;
    }
}
