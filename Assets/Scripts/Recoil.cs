using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    public float recoil_Strength;
    private float recoilTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            recoilTime = 0.1f;

        Recoiling();

        // transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime);
    }

    void Recoiling() {
        if (recoilTime > 0) {
            Quaternion maxRecoil = Quaternion.Euler(0, recoil_Strength, 0);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * 20);
            recoilTime -= Time.deltaTime;
        }
        else {
            recoilTime = 0;
            Quaternion minRecoil = Quaternion.Euler(0, 0, 0);
            
            transform.localRotation = Quaternion.Slerp(transform.localRotation, minRecoil, Time.deltaTime * 10);


        } 
    }
    
    // transform.localRotation = Quaternion.Euler(upRecoil);
}
