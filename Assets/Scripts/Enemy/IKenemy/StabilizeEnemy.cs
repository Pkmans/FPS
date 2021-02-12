using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabilizeEnemy : MonoBehaviour
{
    public float force = 45f;               //default values for easier adjustment
    public float torqueForce = 25f;
    public float maxAngularVel = 0.5f;
    public float thresholdVelocity = 1.5f;

    //the MAIN BODY like hips or torso
    public Rigidbody bodyRb;
    public float rotationForce;

    //grounds
    public float groundCheckRadius = 0.1f;
    private LayerMask whatIsGround;
    
    //number of legs in IKenemy Script
    private IKenemy IKscript;
    private float nLegs;

    // Start is called before the first frame update
    void Start()
    {
        IKscript = GetComponent<IKenemy>();
        whatIsGround = IKscript.whatIsGround;
        nLegs = IKscript.legs.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        StabilizeBodyRotation();
        UpwardForceFromLegs();
        RotateBody();
    }

    void UpwardForceFromLegs() {
        for (int i = 0; i < nLegs; i++) {
            if (Physics.CheckSphere(IKscript.legs[i].transform.position, groundCheckRadius, whatIsGround)) {
                if (bodyRb.velocity.y < thresholdVelocity)
                    bodyRb.AddForce(IKscript.legs[i].transform.up * -force); ///negative force cus orientation of legs is upside down RIP
            }
                // bodyRb.AddForce(legTargetHomes[i].transform.up * -force); 
                //^^using orientation of bone    
        }
    }

    void StabilizeBodyRotation() {
        bodyRb.maxAngularVelocity = maxAngularVel;

        Quaternion rot = Quaternion.FromToRotation(bodyRb.transform.up, Vector3.up);
        bodyRb.AddTorque(new Vector3(rot.x, rot.y, rot.z) * torqueForce, ForceMode.Acceleration);
    }

    void RotateBody() {
        float bodyAngle = bodyRb.transform.eulerAngles.y;
        float desiredAngle = Quaternion.LookRotation(PlayerMovement.instance.transform.position - bodyRb.position, Vector3.up).eulerAngles.y;
        float angleToPlayer = Mathf.DeltaAngle(bodyAngle, desiredAngle);
        
        //clamp angle and use to rotate towards player
        angleToPlayer = Mathf.Clamp(angleToPlayer, -2f, 2f);
        bodyRb.AddTorque(Vector3.up * angleToPlayer * rotationForce);
    }

}
