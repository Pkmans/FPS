using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    //Mouse Sway
    public float amount;
    public float smoothAmount;
    public float maxSwayAmount;
    private Vector3 initialPosition;

    //walk sway
    public float bobAmount;
    private float swayTarget = 1;
    private Vector3 swayPos1, swayPos2;
    private Vector3 bobPosition;

    private Animator anim;
    private PlayerMovement player;

    // Start is called before the first frame update
    void OnEnable()
    {
        initialPosition = transform.parent.localPosition;

        bobPosition = transform.parent.parent.localPosition;
        Vector3 bobFactor = new Vector3(bobAmount, 0, 0);
        swayPos1 = bobPosition - bobFactor;
        swayPos2 = bobPosition + bobFactor;

        // //animator of equipParent
        // anim = transform.parent.parent.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseSway();
    
        if (player.walking)
            WalkSway();
        else 
            ResetPosition();
    }

    void MouseSway()
    {
        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;

        movementX = Mathf.Clamp(movementX, -maxSwayAmount, maxSwayAmount);
        movementY = Mathf.Clamp(movementY, -maxSwayAmount, maxSwayAmount);

        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

    void WalkSway() {
        Vector3 pos = transform.parent.parent.localPosition; ///gameobject equipParent's position

        //first sway point
        if (swayTarget == 1) {
            if (Vector3.Distance (pos, swayPos1) > .1) {
                print ("lerping to target 1");
                transform.parent.parent.localPosition = Vector3.Slerp(pos, swayPos1, Time.deltaTime * 4);
            }
            else swayTarget = 2;
            return;
        }

        //second sway point
        if (Vector3.Distance (pos, swayPos2) > .1)
            transform.parent.parent.localPosition = Vector3.Slerp(pos, swayPos2, Time.deltaTime * 4);
        else swayTarget = 1;

    }

    void ResetPosition() {
        Vector3 pos = transform.parent.parent.localPosition;
        transform.parent.parent.localPosition = Vector3.Lerp(pos, bobPosition, Time.deltaTime * 5);
    }
    
}
