using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float rotationSpeed = 0.1f;
    public bool invert;

    // Update is called once per frame
    void Update()
    {
        if (InputRedirect.pressed && !InputRedirect.inputOverUI)
        {
            Vector3 baseNormal = Vector3.forward;
            Vector3 inputVelocity = new Vector3(InputRedirect.inputVelocity.x, InputRedirect.inputVelocity.y, 0) * rotationSpeed;
            Vector3 newNormal = (baseNormal + inputVelocity).normalized;
            if (invert)
                transform.rotation = Quaternion.FromToRotation(newNormal, baseNormal) * transform.rotation;
            else
                transform.rotation *= Quaternion.FromToRotation(baseNormal, newNormal);
        }
    }
}
