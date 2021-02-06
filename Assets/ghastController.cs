using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghastController : MonoBehaviour
{
    public LegStepper leftLegStepper, rightLegStepper;

    // How fast we can turn and move full throttle
    public float turnSpeed;
    public float moveSpeed;
    // How fast we will reach the above speeds
    public float turnAcceleration;
    public float moveAcceleration;
    // Try to stay in this range from the target
    public float minDistToTarget;
    public float maxDistToTarget;
    // If we are above this angle from the target, start turning
    public float maxAngToTarget;

    private Transform target;
    // World space velocity
    Vector3 currentVelocity;
    // We are only doing a rotation around the up axis, so we only use a float here
    float currentAngularVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(LegUpdateCorourtine());
        target = GameObject.FindWithTag("Player").transform;
    }
    

    // Update is called once per frame
    void LateUpdate()
    {
        RootMotionUpdate();
    }

    void RootMotionUpdate() {
        // Get the direction toward our target
        Vector3 towardTarget = target.position - transform.position;
         // Vector toward target on the local XZ plane
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);

        // Get the angle from the gecko's forward direction to the direction toward toward our target
        // Here we get the signed angle around the up vector so we know which direction to turn in
        float angToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);

        float targetAngularVelocity = 0;

        // If we are within the max angle (i.e. approximately facing the target)
        // leave the target angular velocity at zero
        if (Mathf.Abs(angToTarget) > maxAngToTarget)
        {
            // Angles in Unity are clockwise, so a positive angle here means to our right
            if (angToTarget > 0)
                targetAngularVelocity = turnSpeed;
            // Invert angular speed if target is to our left
            else
                targetAngularVelocity = -turnSpeed;

        }

        // Use our smoothing function to gradually change the velocity
        currentAngularVelocity = Mathf.Lerp(
            currentAngularVelocity,
            targetAngularVelocity,
            1 - Mathf.Exp(-turnAcceleration * Time.deltaTime)
        );

        // Rotate the transform around the Y axis in world space, 
        // making sure to multiply by delta time to get a consistent angular velocity
        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);
    }

    IEnumerator LegUpdateCorourtine() {
        //run continuously
        while (true)
        {
            do 
            {
                leftLegStepper.TryMove();
                yield return null;

                //stay in this loop while leg is moving
                //move other leg if this leg is done moving
            } 
            
            while (leftLegStepper.moving);

            do 
            {
                rightLegStepper.TryMove();
                yield return null;
            } 
            
            while (rightLegStepper.moving);
        }
    }

}
