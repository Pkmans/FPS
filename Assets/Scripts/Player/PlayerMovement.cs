using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Properties")]
    //public variables
    public float moveSpeed = 10f;
    public float maxSpeed;
    public float jumpStrength = 10f;
    private float numOfJumps = 2f;
    public float dashSpeed;
    public float dashDuration = 0.15f;
    public float turnSpeed;
    public float extraGravity;

    // public float DEFAULT_DRAG;
    [Header("Drag")]
    public float AIR_DRAG;
    public float COUNTER_DRAG;
    public float SLIDING_DRAG;

    [Header("GroundCheck")]
    //groundchecks
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("E.t.c.")]
    public ParticleSystem dashParticles;

    //script references
    private SlideMovement slideScript;
    private bool isSliding;

    //global velocity vars
    private Vector2 vel;
    private Rigidbody rb;

    [HideInInspector]
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
        isSliding = slideScript.isSliding;

        if (Input.GetButtonDown("Jump")) Jump(); 

        if (grounded && numOfJumps < 2)
            numOfJumps = 2;

        if (Input.GetKeyDown(KeyCode.LeftShift))
            StartCoroutine(Dash());

        //check if player walking on ground
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //bool for sliding script
        if (grounded && (x != 0 || z != 0))
            walking = true;
        else walking = false;
    }
    
    void FixedUpdate() {
        rb.AddForce(Vector3.down * extraGravity);

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

        //normalize diagonal speed and remove input if sliding
        Vector2 flatVel = new Vector2(rb.velocity.x, rb.velocity.z);
        if (Mathf.Abs(flatVel.magnitude) > maxSpeed || isSliding) {
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
        if (numOfJumps == 0) return;

        if (grounded) {
            rb.AddForce(transform.up * jumpStrength);
            numOfJumps--;
        } else {
            Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.velocity = vel;
            rb.AddForce(transform.up * jumpStrength);
            numOfJumps = 0;
        }

    }

    void CounterMovement(float x, float z) {
        bool noInputs = x == 0 & z == 0;

        //DRAG FOR SLIDING
        if (grounded && isSliding) {
            Drag(SLIDING_DRAG);
            return;
        } else if (!grounded && isSliding) {
            Drag(0);
            return;
        }
            
        //DRAG FOR GROUND, MID-AIR, AND DEFAULT
        if (grounded && noInputs)
            Drag(COUNTER_DRAG);
        else if (!grounded && noInputs)
            Drag(AIR_DRAG);
        else Drag(0);
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
    IEnumerator Dash() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        float xVel = vel.x;
        float zVel = vel.y;

        dashParticles.Play();

        //default dash forward if no inputs
        if (x == 0 & z == 0) rb.velocity = transform.forward * dashSpeed;
            
        //diagonal dash directions
        if (z > 0 && x > 0) rb.velocity = (transform.forward + transform.right).normalized * dashSpeed;
        else if (z > 0 && x < 0) rb.velocity = (transform.forward - transform.right).normalized * dashSpeed;
        else if (z < 0 && x < 0) rb.velocity = (-transform.forward - transform.right).normalized * dashSpeed;
        else if (z < 0 && x > 0) rb.velocity = (-transform.forward + transform.right).normalized * dashSpeed;
        //choose dash direction (left right forward backward)
        else if (z > 0) rb.velocity = transform.forward * dashSpeed;
        else if (z < 0) rb.velocity = -transform.forward * dashSpeed;
        else if (x > 0) rb.velocity = transform.right * dashSpeed;
        else if (x < 0) rb.velocity = -transform.right * dashSpeed;

        rb.useGravity = false;

        yield return new WaitForSeconds(dashDuration); //dash duration

        EndDash();
    }

    void EndDash() {
        dashParticles.Stop();
        rb.useGravity = true;
        
        float reducedSpeed = maxSpeed + 5;

        Vector3 diagonalVector = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 clampedVector = Vector3.ClampMagnitude(diagonalVector, reducedSpeed);

        rb.velocity = clampedVector + new Vector3(0, rb.velocity.y, 0);
    }

    public void KnockBack(Vector3 knockDirection) {
        rb.AddForce(knockDirection, ForceMode.Impulse);
    }

    ///add force method
    // IEnumerator Dash() {
    //     float x = Input.GetAxisRaw("Horizontal");
    //     float z = Input.GetAxisRaw("Vertical");

    //     //zero out velocity except y-axis
    //     Vector3 resetVel = new Vector3(0, rb.velocity.y, 0);
    //     rb.velocity = resetVel;

    //     //choose dash direction
    //     if (z > 0) rb.AddForce(transform.forward * dashSpeed, ForceMode.Impulse);
    //     if (z < 0) rb.AddForce(-transform.forward * dashSpeed, ForceMode.Impulse);
    //     if (x > 0) rb.AddForce(transform.right * dashSpeed, ForceMode.Impulse);
    //     if (x < 0) rb.AddForce(-transform.right * dashSpeed, ForceMode.Impulse);

    //     yield return new WaitForSeconds(0.00001f);
  
    //     dashing = false;
    // }


    


}
