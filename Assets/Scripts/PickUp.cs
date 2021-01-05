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

    private Recoil recoil;

    // Start is called before the first frame update
    void Start()
    {
        recoil = GetComponent<Recoil>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrab();

        if (Input.GetKeyDown(KeyCode.E) && canGrab) 
            pickingUp = true;
        
        if (pickingUp)
            PickUpWeapon();

        if (Input.GetKeyDown(KeyCode.G) && currentWeapon)
            DropWeapon();
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

    void PickUpWeapon() {
        //smoothly move weapon to equip Position
        currentWeapon.transform.parent = equipPos;
        currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, equipPos.position, Time.deltaTime * 9);
        currentWeapon.transform.rotation = Quaternion.Slerp(currentWeapon.transform.rotation, equipPos.rotation, Time.deltaTime * 9);
        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;

        
        

        if (Vector3.Distance(currentWeapon.transform.position, equipPos.position) <= 0.1f) {
            pickingUp = false;
            currentWeapon.GetComponent<Gun>().enabled = true;
            currentWeapon.GetComponent<WeaponSway>().enabled = true;
        }
    }

    void DropWeapon() {
        currentWeapon.transform.parent = null;
        currentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        currentWeapon.GetComponent<Gun>().enabled = false;

        currentWeapon = null;
        pickingUp = false;
    }
}
