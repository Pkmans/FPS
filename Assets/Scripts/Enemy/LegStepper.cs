using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    public Transform homeTransform;
    public float wantStepAtDistance;
    public float moveDuration; //how long a step takes to complete
    public float stepOvershootFraction;

    public bool moving;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryMove() {
        if (moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        //if too far off in position or rotation
        if (distFromHome > wantStepAtDistance)
            StartCoroutine(MoveToHome());
    }

    IEnumerator MoveToHome() {
        moving = true;

        //store initial conditions
        Quaternion startRot = transform.rotation;
        Vector3 startPos = transform.position;

        Quaternion endRot = homeTransform.rotation;
        Vector3 towardHome = (homeTransform.position - transform.position);

        float overShootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overShootVector = towardHome.normalized * overShootDistance;

        //restrict overshootvector to be level with ground
        overShootVector = Vector3.ProjectOnPlane(overShootVector, Vector3.up);

        // Vector3 endPos = homeTransform.position + overShootVector; 
        ///change
        Vector3 endPos = homeTransform.position + overShootVector;


        //leg moves up, passing center point
        Vector3 centerPoint = (startPos + endPos)/2;
        centerPoint += Vector3.up * Vector3.Distance(startPos, endPos) / 2.5f;

        float timeElapsed = 0;

        do //do-while loop checks cond at end of loop
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.Cubic.InOut(normalizedTime);

            //Quadratic bezier curve
            transform.position = 
              Vector3.Lerp(
                Vector3.Lerp(startPos, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPos, normalizedTime),
                normalizedTime
              );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            //wait for one frame 
            yield return null;
        } while (timeElapsed < moveDuration);

        //done moving
        moving = false;
    }

}
