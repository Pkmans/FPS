﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMovement : MonoBehaviour
{
    //public variables
    public float launchSpeed;
    public float angleDivisor;
    public float tiltAngle;
    public float tiltSmooth;

    //local variables for methods
    public Transform crouchPosition;
    private Vector3 slideDir;

    //referenced by movement script
    public bool isSliding;

    //component & script refs
    private Rigidbody rb;
    public CapsuleCollider innerCol, outerCol;
    private PlayerMovement movementScript;

    //camera tilt
    public GameObject cam;
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementScript = GetComponent<PlayerMovement>();

        initialRotation = cam.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Slide();

        if (isSliding) Tilt();
        else ResetTilt();

    }

    void Slide() {
        if (Input.GetKey(KeyCode.LeftControl))
            StartSlide();
        else FinishSlide();
    }

    void FixedUpdate() {
        Sliding();
    }

    void StartSlide() {
        if (isSliding) return;
        movementScript.maxSpeed += 9f;

        slideDir = transform.forward;

        innerCol.height /= 2;
        outerCol.height /= 2;

        transform.position = crouchPosition.position;

        //boost in speed when starting slide
        rb.AddForce(slideDir * launchSpeed * 1.5f);
        isSliding = true;

        AudioManager.instance.Play("sliding");
    }
    
    void Sliding() {
        if (!movementScript.grounded || !isSliding) return;

        Vector3 slopeDirection = Vector3.zero;

        //calculate angle of slope
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, innerCol.height/2 * 3f))
            slopeDirection = hit.normal;
    
        float groundAngle = Vector3.Angle(Vector3.up, slopeDirection);

        //steeper slope, faster speed
        rb.AddForce(slideDir * groundAngle / angleDivisor);
        rb.AddForce(Vector3.down * 10f);
    }

    void FinishSlide() {
        if (!isSliding) return;

        innerCol.height *= 2;
        outerCol.height *= 2;

        isSliding = false;
        slideDir = Vector3.zero;

        AudioManager.instance.Stop("sliding");

        movementScript.maxSpeed -= 9f;
    }

    void changeHeight(float newHeight) {
        Vector3 newScale = transform.localScale;
        newScale.y = newHeight;

        transform.localScale = newScale;
    }

    void Tilt() {
        Vector2 vel = movementScript.CurVelocityRelativeToLook();
        float xVel = vel.x;
        float zVel = vel.y;


        if (!angleSet) {
            if (xVel < 0) 
                tiltRotation = Quaternion.Euler(0, 0, -tiltAngle);
            else 
                tiltRotation = Quaternion.Euler(0, 0, tiltAngle);

            angleSet = true;
        }
        
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, tiltRotation, Time.deltaTime * tiltSmooth);
    }

    void ResetTilt() {
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, initialRotation, Time.deltaTime * tiltSmooth);
        angleSet = false;
    }

    private Quaternion tiltRotation;
    private bool angleSet;
}
