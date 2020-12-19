using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public variables
    public float moveSpeed = 10f;
    public float maxSpeed;
    public float jumpStrength = 10f;
    public float extraGravity;
    public float DEFAULT_DRAG = 0f;
    public float COUNTER_DRAG = 3f;

    private Rigidbody rb;

    //groundchecks
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    public bool grounded;

    //script references
    private SlideMovement slideScript;
    private bool isSliding;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        slideScript = GetComponent<SlideMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, checkRadius, whatIsGround);
        bool jumping = Input.GetButtonDown("Jump");
        isSliding = slideScript.isSliding;

        if (grounded && jumping) 
            Jump();

    }
    
    void FixedUpdate() {
        rb.AddForce(Vector3.down * extraGravity);

        if (isSliding) return;
        Move();
    }

    void Move() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //vel relative to look direction
        Vector2 vel = CurVelocityRelativeToLook();
        float xVel = vel.x;
        float zVel = vel.y;

        //no input if exceeding max speed
        if (x > 0 && xVel > maxSpeed) x = 0;
        if (x < 0 && xVel < -maxSpeed) x = 0;
        if (z > 0 && zVel > maxSpeed) z = 0;
        if (z < 0 && zVel < -maxSpeed) z = 0;

        CounterMovement(x, z);

        //move forward and sideways
        rb.AddForce(transform.forward * z * moveSpeed);
        rb.AddForce(transform.right * x * moveSpeed);

    }

    void Jump() {
        rb.AddForce(transform.up * jumpStrength);
    }


    void CounterMovement(float x, float z) {
        if (grounded && x == 0 && z == 0)
            rb.drag = COUNTER_DRAG;
        else
            rb.drag = DEFAULT_DRAG;
    }


    Vector2 CurVelocityRelativeToLook() {
        float xVel = transform.InverseTransformDirection(rb.velocity).x;
        float zVel = transform.InverseTransformDirection(rb.velocity).z;

        return new Vector2(xVel, zVel);
    }


}
