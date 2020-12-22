using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{

    private Rigidbody MainRb;
    private Collider MainCollider;

    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        MainRb = GetComponent<Rigidbody>();
        MainCollider = GetComponent<Collider>();

        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        ToggleRagdoll(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleRagdoll (bool on) {
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = !on;
        }

        foreach (Collider col in colliders) {
            col.enabled = on;
        }

        MainRb.useGravity = !on;
        MainCollider.enabled = !on;
    }
}
