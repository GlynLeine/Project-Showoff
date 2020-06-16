using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float maxRotationSpeed = 0.1f;
    public float minRotationSpeed = 0.001f;
    public float maxScale = 0.001f;
    public float minScale = 0.0004f;
    public bool invert;
    public Zoom zoom;
    public float range;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        range = GameManager.smoothstep(zoom.zoomSlider.minValue, zoom.zoomSlider.maxValue, zoom.zoomSlider.value);

        if (InputRedirect.pressed && !InputRedirect.inputOverUI)
        {
            Vector3 baseNormal = Vector3.forward;
            Vector3 inputVelocity = new Vector3(InputRedirect.inputVelocity.x, InputRedirect.inputVelocity.y, 0) * GameManager.lerp(minScale, maxScale, range);

            float maxSpeed = GameManager.lerp(minRotationSpeed, maxRotationSpeed, range);
            if (inputVelocity.magnitude > maxSpeed)
                inputVelocity = inputVelocity.normalized * maxSpeed;

            speed = inputVelocity.magnitude;

            Vector3 newNormal = (baseNormal + inputVelocity).normalized;
            if (invert)
                transform.rotation = Quaternion.FromToRotation(newNormal, baseNormal) * transform.rotation;
            else
                transform.rotation *= Quaternion.FromToRotation(baseNormal, newNormal);
        }
    }
}
