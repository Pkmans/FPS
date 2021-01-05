using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public variables
    public float moveSpeed = 10f;
    public float maxSpeed;
    public float jumpStrength = 10f;
    public float dashSpeed;
    public float turnSpeed;
    public float extraGravity;
    public float DEFAULT_DRAG = 0f;
    public float AIR_DRAG = 0f;
    public float COUNTER_DRAG = 3f;

    //global velocity vars
    Vector2 vel;

    //groundchecks
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    public bool grounded;

    //script references
    private SlideMovement slideScript;
    private bool isSliding;

    //dashing
    private bool dashing;

    private Rigidbody rb;

    // [HideInInspector]
    public bool walking;

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

        if (Input.GetKeyDown(KeyCode.LeftShift))
            StartCoroutine(Dash());

        //check if player walking on ground
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (grounded && (x != 0 || z != 0))
            walking = true;
        else walking = false;
    }
    
    void FixedUpdate() {
        rb.AddForce(Vector3.down * extraGravity);

        if (isSliding) return;
        // if (dashing) return;

        vel = CurVelocityRelativeToLook();
        Move();
    }

    void Move() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //vel relative to look direction
        float xVel = vel.x;
        float zVel = vel.y;

        //no input if exceeding max speed
        if (x > 0 && xVel > maxSpeed) x = 0;
        if (x < 0 && xVel < -maxSpeed) x = 0;
        if (z > 0 && zVel > maxSpeed) z = 0;
        if (z < 0 && zVel < -maxSpeed) z = 0;

        //normalize diagonal speed
        Vector2 flatVel = new Vector2(rb.velocity.x, rb.velocity.z);
        if (Mathf.Abs(flatVel.magnitude) > maxSpeed) {
            x = 0;
            z = 0;
        }

        CounterMovement(x, z);

        //move forward and sideways
        rb.AddForce(transform.forward * z * moveSpeed);
        rb.AddForce(transform.right * x * moveSpeed);

        //change direction of player faster
        if (zVel > 0.2f && z < 0) rb.AddForce(-transform.forward * turnSpeed);   
        if (zVel < -0.2f && z > 0) rb.AddForce(transform.forward * turnSpeed);  
        if (xVel > 0.2f && x < 0) rb.AddForce(-transform.right * turnSpeed);   
        if (xVel < -0.2f && x > 0) rb.AddForce(transform.right * turnSpeed);  

    }
    
    void Jump() {
        rb.AddForce(transform.up * jumpStrength);
    }

    void CounterMovement(float x, float z) {
        bool noInputs = x == 0 & z == 0;

        if (grounded && noInputs)
            Drag(COUNTER_DRAG);
        else if (!grounded && noInputs)
            Drag(AIR_DRAG);
        else
            rb.drag = DEFAULT_DRAG;
    }

    void Drag(float dragStrength) {
        float xVel = vel.x;
        float zVel = vel.y;

        if (zVel > 0.2f) rb.AddForce(-transform.forward * dragStrength);
        if (zVel < -0.2f) rb.AddForce(transform.forward * dragStrength);
        if (xVel > 0.2f) rb.AddForce(-transform.right * dragStrength);
        if (xVel < -0.2f) rb.AddForce(transform.right * dragStrength);
    }


    Vector2 CurVelocityRelativeToLook() {
        float xVel = transform.InverseTransformDirection(rb.velocity).x;
        float zVel = transform.InverseTransformDirection(rb.velocity).z;

        return new Vector2(xVel, zVel);
    }

    ///set velocity method
    // IEnumerator Dash() {
    //     float x = Input.GetAxisRaw("Horizontal");
    //     float z = Input.GetAxisRaw("Vertical");

    //     //choose dash direction
    //     if (z > 0) rb.velocity = transform.forward * dashSpeed;
    //     if (z < 0) rb.velocity = -transform.forward * dashSpeed;
    //     if (x > 0) rb.velocity = transform.right * dashSpeed;
    //     if (x < 0) rb.velocity = -transform.right * dashSpeed;

    //     rb.useGravity = false;

    //     float temp = COUNTER_DRAG;
    //     COUNTER_DRAG = 0f;

    //     yield return new WaitForSeconds(0.2f);

    //     COUNTER_DRAG = temp;

    //     StopDash();
    // }

    ///add force method
    IEnumerator Dash() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //zero out velocity except y-axis
        Vector3 resetVel = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = resetVel;

        //choose dash direction
        if (z > 0) rb.AddForce(transform.forward * dashSpeed, ForceMode.Impulse);
        if (z < 0) rb.AddForce(-transform.forward * dashSpeed, ForceMode.Impulse);
        if (x > 0) rb.AddForce(transform.right * dashSpeed, ForceMode.Impulse);
        if (x < 0) rb.AddForce(-transform.right * dashSpeed, ForceMode.Impulse);

        yield return new WaitForSeconds(0.00001f);
  
        dashing = false;
    }


    void StopDash() {
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        dashing = false;
    }


}
