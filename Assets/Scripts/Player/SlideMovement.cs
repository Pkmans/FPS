using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMovement : MonoBehaviour
{
    //public variables
    public float speed;
    public float reducedHeight = 0.5f;
    private float origHeight;

    //local variables for methods
    private Vector3 slideDir;

    //referenced by movement script
    public bool isSliding;

    //component references
    private Rigidbody rb;
    private CapsuleCollider col;

    //script reference
    private PlayerMovement movementScript;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementScript = GetComponent<PlayerMovement>();
        col = GetComponent<CapsuleCollider>();

        origHeight = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        Slide();
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

        changeHeight(reducedHeight);
        rb.AddForce(slideDir * speed);
        isSliding = true;
    }
    
    void Sliding() {
        if (CheckIfOnSlope()) {
            rb.AddForce(slideDir * 10f);

            //downward force to keep player on slope
            rb.AddForce(Vector3.down * 10f);
        }
            

    }

    void FinishSlide() {
        changeHeight(origHeight);
        isSliding = false;
        slideDir = Vector3.zero;
    }
    
    bool CheckIfOnSlope() {
        if (!movementScript.grounded) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, col.height/2 * 3f))
            return (Vector3.Distance(hit.normal, Vector3.up) > .1f);
        else
            return false;
    }

    void changeHeight(float newHeight) {
        Vector3 newScale = transform.localScale;
        newScale.y = newHeight;

        transform.localScale = newScale;
    }
}
