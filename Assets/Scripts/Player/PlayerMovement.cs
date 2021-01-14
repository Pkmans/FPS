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

    [Header("WallRunning")]
    public float tiltAngle;
    public float tiltSmooth;
    public float wallrunForce, wallHopForce;
    public float speedMultiplier = 1.3f;
    private bool isWallRight, isWallLeft, isWallFront, isWallBack;
    private bool isWallRunning;
    private bool touchingWall;
    private Quaternion initialRotation;

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
    public Animator anim;
    public GameObject cam;
    
    //script references
    private SlideMovement slideScript;
    private bool isSliding;
    private VisualEffects visualFX;

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
        visualFX = GetComponent<VisualEffects>();

        initialRotation = cam.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, checkRadius, whatIsGround);
        isSliding = slideScript.isSliding;

        if (isWallRunning && isWallRight) 
            TiltCamera(1);
        else if (isWallRunning && isWallLeft)
            TiltCamera(-1);
        else ResetTilt();

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
        //extra downward force
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration); ///dis correct

        CheckForWall();
        if (!grounded && touchingWall && !isWallRunning)
            StartWallRun();
        WallRun();

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

        anim.enabled = true;
        anim.SetTrigger("jump");

        ///wallrunning jumps
        if (isWallRunning) {
            //wall hop
            // if (isWallRight && Input.GetKey(KeyCode.A)) rb.AddForce(-transform.right * wallHopForce);
            // if (isWallLeft && Input.GetKey(KeyCode.D)) rb.AddForce(transform.right * wallHopForce);
            // if (isWallFront && Input.GetKey(KeyCode.S)) rb.AddForce(-transform.forward * wallHopForce * 1.13f);
            // if (isWallBack && Input.GetKey(KeyCode.W)) rb.AddForce(transform.forward * wallHopForce * 1.13f);
            if (isWallRight) rb.AddForce(-transform.right * wallHopForce);
            if (isWallLeft) rb.AddForce(transform.right * wallHopForce);
            if (isWallFront) rb.AddForce(-transform.forward * wallHopForce * 1.13f);
            if (isWallBack) rb.AddForce(transform.forward * wallHopForce * 1.13f);

            //little bit of forward force
            // rb.AddForce(transform.forward * jumpStrength / 2);
        }

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


    public Vector2 CurVelocityRelativeToLook() {
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
        dashParticles.Play();
        visualFX.dashFX(dashDuration);

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


    ///
    ///WALLRUNNING SCRIPT
    ///
    void OnCollisionEnter(Collision col) {
        if (Mathf.Abs(Vector3.Dot(col.contacts[0].normal, Vector3.up)) < 0.1f)
            touchingWall = true;
    }

    void StartWallRun() {
        //small vertical boost at start of wall run
        if (rb.velocity.y < 2f) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * 300f);
        }
        
    
        rb.useGravity = false;
        isWallRunning = true;
        maxSpeed *= speedMultiplier;

        if (numOfJumps <= 0) 
            numOfJumps = 1;
    }

    private void WallRun() {
        if (!isWallRunning) return;

        //make sure player sticks to the wall
        if (isWallRight) rb.AddForce(transform.right * wallrunForce * 1.3f);
        if (isWallLeft) rb.AddForce(-transform.right * wallrunForce * 1.3f);
        if (isWallFront) rb.AddForce(transform.forward * wallrunForce * 1.3f);
        if (isWallBack) rb.AddForce(-transform.forward * wallrunForce * 1.3f);
    }

    private void StopWallRun() {
        numOfJumps = 1;

        rb.useGravity = true;
        isWallRunning = false;
        maxSpeed /= speedMultiplier;
    }

    private void CheckForWall() {
        ///changes
        RaycastHit hit;

        //checks if there's a wall on all four sides of player
        if (Physics.Raycast(transform.position, transform.right, out hit, .8f, whatIsGround))
            isWallRight = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) < 0.1f;
        else isWallRight = false;
        if (Physics.Raycast(transform.position, -transform.right, out hit, .8f, whatIsGround))
            isWallLeft = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) < 0.1f;
        else isWallLeft = false;
        if (Physics.Raycast(transform.position, transform.forward, out hit, .8f, whatIsGround))
            isWallFront = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) < 0.1f;
        else isWallFront = false;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, .8f, whatIsGround))
            isWallBack = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) < 0.1f;
        else isWallBack = false;


        if (!isWallFront && !isWallBack && !isWallLeft && !isWallRight)
            touchingWall = false;

        //leave wall run if no walls, or if grounded
        if (isWallRunning && (grounded || !touchingWall))
            StopWallRun();
    }

    void TiltCamera(float direction) {
        Quaternion tiltRotation;

        //direction > 0 is tilt right, direction < 0 is tilt left
        if (direction > 0) tiltRotation = Quaternion.Euler(0, 0, tiltAngle);
        else tiltRotation = Quaternion.Euler(0, 0, -tiltAngle);

        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, tiltRotation, Time.deltaTime * tiltSmooth);
    }

    void ResetTilt() {
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, initialRotation, Time.deltaTime * (tiltSmooth - 0.4f));
    }


}
