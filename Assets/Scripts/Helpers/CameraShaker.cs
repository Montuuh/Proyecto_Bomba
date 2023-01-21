using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{

    // Desired duration of the shake effect
    private float shakeDuration = 0f;

    // A measure of magnitude for the shake. Tweak based on your preference
    private float shakeMagnitude = 0.7f;

    // A measure of how quickly the shake effect should evaporate
    private float dampingSpeed = 1.0f;

    // The initial position of the GameObject
    Vector3 initialPosition;

    public bool active = false;

    void Update()
    {
        // Displace camera in random ways and then return to origin
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else if(active)
        {
            active = false;
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }


    public void TriggerShake(float duration, float intensity)
    {
        initialPosition = transform.localPosition;
        shakeDuration = duration;
        shakeMagnitude = intensity;
        active = true;
    }

    // Default shake
    public void TriggerStandardShake()
    {
        initialPosition = transform.localPosition;
        shakeDuration = 1f;
        shakeMagnitude = 20f;
        active = true;
    }
}
