using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRedirect : MonoBehaviour
{
    static private Vector2 previousInputPos;
    static public Vector2 inputPos;
    static public Vector2 inputVelocity;
    static public bool pressed;
    private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            inputPos = touch.position;
            previousInputPos = inputPos;
            inputVelocity = touch.deltaPosition;

            pressed = true;
        }
        else
        {
            previousInputPos = inputPos;
            inputPos = Input.mousePosition;
            inputVelocity = inputPos - previousInputPos;
            pressed = Input.GetMouseButton(0);
        }
    }
}