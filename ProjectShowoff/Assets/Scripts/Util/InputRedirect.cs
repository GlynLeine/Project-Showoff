using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputRedirect : MonoBehaviour
{
    public EventSystem eventSystem;
    public GraphicRaycaster raycaster;
    public PlanetReset planetResetScript;

    static private Vector2 previousInputPos;
    static private float previousTouchZoom;
    static private bool touchZoomReset;

    static public Vector2 inputPos;
    static public Vector2 inputVelocity;
    static public bool pressed;
    static public bool tapped;
    static public float zoom;

    static public bool inputOverUI;

    private float timer = 0;

    private void Update()
    {
        //Set up the new Pointer Event
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        //Set the Pointer Event Position to that of the mouse position
        pointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        raycaster.Raycast(pointerEventData, results);

        inputOverUI = results.Count > 0;
        timer += Time.deltaTime;
        
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            inputPos = touch.position;
            previousInputPos = inputPos;
            inputVelocity = touch.deltaPosition;

            if(!pressed)
                tapped = true;
            else
                tapped = false;

            pressed = true;
            touchZoomReset = true;
            timer = 0;
        }
        else if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);
            timer = 0;

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

            tapped = Input.GetMouseButtonDown(0);
            pressed = Input.GetMouseButton(0);
            zoom += Input.GetAxis("Mouse ScrollWheel");
        }

        if (inputPos != previousInputPos)
        {
            timer = 0;
        }

        if (timer > 20)
        {
            //planetResetScript.ResetOnNoInteract();
            timer = 0;
        }
    }
}