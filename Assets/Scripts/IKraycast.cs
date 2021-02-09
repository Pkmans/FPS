using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKraycast : MonoBehaviour
{
    public Transform homeTransform;
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 5f, whatIsGround)) {
            homeTransform.position = hit.point;
        }
    }
}
