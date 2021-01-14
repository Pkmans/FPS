using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class VisualEffects : MonoBehaviour
{
    public Camera cam;

    //post process
    public PostProcessVolume volume;
    private ChromaticAberration chromatic;
    private LensDistortion lensDistortion;
    
    //dashFX
    public float dashSmooth;
    public float disortValue;
    public float chromaValue;
    public float valueFOV;
    private bool dash;
    private bool offDash;
    private float originalValue;

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGetSettings(out chromatic);
        volume.profile.TryGetSettings(out lensDistortion);

        originalValue = chromatic.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (dash) {
            chromatic.intensity.value = Mathf.Lerp(chromatic.intensity.value, chromaValue, Time.deltaTime * dashSmooth);
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, disortValue, Time.deltaTime * dashSmooth);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, valueFOV, Time.deltaTime * dashSmooth);
        }
            
        
        if (offDash) {
            chromatic.intensity.value = Mathf.Lerp(chromatic.intensity.value, originalValue, Time.deltaTime * dashSmooth);
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, 0, Time.deltaTime * dashSmooth);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 80, Time.deltaTime * dashSmooth);

        }
    }

    public void dashFX(float duration) {
        dash = true;
        Invoke("turnOffDash", duration);
    }

    void turnOffDash() { 
        dash = false;
        offDash = true;
    }
}
