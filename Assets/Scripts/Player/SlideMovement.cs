using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMovement : MonoBehaviour
{
    //public variables
    public float speed;

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
    public float tiltSmooth;
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

    void FixedUpdate() {

    }

    void Slide() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            StartSlide();
            Sliding();
        } 
        else FinishSlide();
    }

    void StartSlide() {
        if (isSliding) return;

        slideDir = transform.forward;

        innerCol.height /= 2;
        outerCol.height /= 2;

        transform.position = crouchPosition.position;

        rb.AddForce(slideDir * speed);
        isSliding = true;

        AudioManager.instance.Play("sliding");
    }
    
    void Sliding() {
        if (CheckIfOnSlope()) {
            rb.AddForce(slideDir * 10f);

            //downward force to keep player on slope
            rb.AddForce(Vector3.down * 10f);
        }
    }

    void FinishSlide() {
        if (!isSliding) return;

        innerCol.height *= 2;
        outerCol.height *= 2;

        isSliding = false;
        slideDir = Vector3.zero;

        AudioManager.instance.Stop("sliding");

    }
    
    bool CheckIfOnSlope() {
        if (!movementScript.grounded) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, innerCol.height/2 * 3f))
            return (Vector3.Distance(hit.normal, Vector3.up) > .1f);
        else
            return false;
    }

    void changeHeight(float newHeight) {
        Vector3 newScale = transform.localScale;
        newScale.y = newHeight;

        transform.localScale = newScale;
    }

    void Tilt() {
        Quaternion tiltRotation = Quaternion.Euler(0, 0, 5);
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, tiltRotation, Time.deltaTime * tiltSmooth);
    }

    void ResetTilt() {
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, initialRotation, Time.deltaTime * tiltSmooth);
    }

    
}
