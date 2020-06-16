using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRotator : MonoBehaviour
{
    public GameObject cameraRotationObject;
    public Zoom zoomScript;
    private TutorialScript tutorialScript;
    private bool allTrue;
    void Start()
    {
        tutorialScript = gameObject.GetComponent<TutorialScript>();
        StartCoroutine(zoomDone());
    }

    IEnumerator zoomDone()
    {
        while (!allTrue)
        {
            Vector3 tempRotation = cameraRotationObject.transform.eulerAngles;
            //i cant fucking get it to work so lets move on for a sec
            allTrue = true;
            zoomScript.range = 2;
            tempRotation.x = 26;
            tempRotation.y = 57;
            tempRotation.z = 7;
            /*if (tempRotation.x > 26)
            {
                tempRotation.x -= 100 * Time.deltaTime;
                if (tempRotation.x <= 26)
                {
                    tempRotation.x = 26;
                }
            }
            else if (tempRotation.x < 26)
            {
                tempRotation.x += 100 * Time.deltaTime;
                if (tempRotation.x >= 26)
                {
                    tempRotation.x = 26;
                }
                else if (tempRotation.x < 0)
                {
                    tempRotation.x = 0;
                }
            }
            if (tempRotation.y > 57)
            {
                tempRotation.y -= 100 * Time.deltaTime;
                if (tempRotation.y <= 57)
                {
                    tempRotation.y = 57;
                }
            }
            else if (tempRotation.y < 57)
            {
                tempRotation.y += 100 * Time.deltaTime;
                if (tempRotation.y >= 57)
                {
                    tempRotation.y = 57;
                }
            }
            /*if (tempRotation.z > 7)
            {
                tempRotation.z -= 100 * Time.deltaTime;
                if (tempRotation.z <= 7)
                {
                    tempRotation.z = 7;
                }
            }
            else if (tempRotation.z < 7)
            {
                tempRotation.z += 100 * Time.deltaTime;
                if (tempRotation.z >= 7)
                {
                    tempRotation.z = 7;
                }
            }#1#
            if (zoomScript.range > 2)
            {
                zoomScript.range -= 8 * Time.deltaTime;
                if (zoomScript.range <= 2)
                {
                    zoomScript.range = 2;
                }
            }
            else if (zoomScript.range < 2)
            {
                zoomScript.range += 8 * Time.deltaTime;
                if (zoomScript.range >= 2)
                {
                    zoomScript.range = 2;
                }
            }*/

            cameraRotationObject.transform.eulerAngles = tempRotation;
            yield return null;
        }
        tutorialScript.tutorialStart();
    }
}
