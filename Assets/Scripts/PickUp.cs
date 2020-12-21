using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float distance;
    public Transform cam;
    public Transform equipPos;

    private GameObject currentWeapon;
    private bool canGrab;
    private bool pickingUp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrab();

        if (Input.GetKeyDown(KeyCode.E) && canGrab) 
            pickingUp = true;
        
        if (pickingUp)
            PickUpGun();
    }

    void CheckGrab() {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, distance)) {
            if (hit.transform.gameObject.tag == "canGrab") {
                currentWeapon = hit.transform.gameObject;
                canGrab = true;
            }    
        }
        else canGrab = false;
    }

    void PickUpGun() {
        print("gun picked up");

        //smoothly move weapon to equip Position
        currentWeapon.transform.parent = equipPos;
        currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, equipPos.position, Time.deltaTime * 9);
        currentWeapon.transform.rotation = Quaternion.Slerp(currentWeapon.transform.rotation, equipPos.rotation, Time.deltaTime * 9);
        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;

        currentWeapon.GetComponent<Gun>().enabled = true;

        if (currentWeapon.transform.position == equipPos.position) {
            pickingUp = false;
        }
            

    }
}
