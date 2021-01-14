using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float distance;
    public Transform cam;
    public Transform equipPos;

    private GameObject currentWeapon;
    private GameObject weaponInSight;
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

        if (Input.GetKeyDown(KeyCode.E) && canGrab && !currentWeapon) {
            pickingUp = true;
            currentWeapon = weaponInSight;
        }
            
        
        if (pickingUp)
            PickUpWeapon();

        if (Input.GetKeyDown(KeyCode.G) && currentWeapon)
            DropWeapon();
    }

    void CheckGrab() {
        RaycastHit hit;

        Debug.DrawRay(cam.position, cam.forward*distance, Color.red);

        if (Physics.Raycast(cam.position, cam.forward, out hit, distance)) {
            if (hit.transform.gameObject.tag == "canGrab") {
                weaponInSight = hit.transform.gameObject;
                canGrab = true;
            }
            else {
            weaponInSight = null;
            canGrab = false;
            }
        } 
            
    }

    void PickUpWeapon() {
        //smoothly move weapon to equip Position
        currentWeapon.transform.parent = equipPos;
        currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, equipPos.position, Time.deltaTime * 9);
        currentWeapon.transform.rotation = Quaternion.Slerp(currentWeapon.transform.rotation, equipPos.rotation, Time.deltaTime * 9);
        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;

       
        currentWeapon.layer = 14;   //set layer to 'localGun' as well as its children objects
        foreach (Transform t in currentWeapon.transform) {
            t.gameObject.layer = 14;
        }


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
        currentWeapon.GetComponent<WeaponSway>().enabled = false;

        currentWeapon.layer = 0;
        foreach (Transform t in currentWeapon.transform) {
            t.gameObject.layer = 0; //set layer back to 'default'
        }

        currentWeapon = null;
        pickingUp = false;
    }
}
