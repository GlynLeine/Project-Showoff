using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRedirect : MonoBehaviour
{
    static private Vector2 previousInputPos;
    static private float previousTouchZoom;
    static private bool touchZoomReset;

    static public Vector2 inputPos;
    static public Vector2 inputVelocity;
    static public bool pressed;
    static public float zoom;

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            inputPos = touch.position;
            previousInputPos = inputPos;
            inputVelocity = touch.deltaPosition;

            pressed = true;
            touchZoomReset = true;
        }
        else if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);

            if (touchZoomReset)
            {
                previousTouchZoom = distance;
                touchZoomReset = false;
            }
            else
            {
                zoom += distance - previousTouchZoom;
                previousTouchZoom = distance;
            }
        }
        else
        {
            touchZoomReset = true;
            previousInputPos = inputPos;
            inputPos = Input.mousePosition;
            inputVelocity = inputPos - previousInputPos;
            pressed = Input.GetMouseButton(0);
            zoom += Input.GetAxis("Mouse ScrollWheel");
        }
    }
}