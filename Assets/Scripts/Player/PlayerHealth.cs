using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 1;

    public CapsuleCollider innerCol, outerCol;
    public GameObject deathMenu;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) 
            Die();
    }

    void Die() {
        KnockDownPlayer();
        
        //open up death screen
        GameManager.instance.PlayerDeath(); 
    }

    void KnockDownPlayer() {
        innerCol.height = 0.1f;
        innerCol.radius = 0.1f;
        innerCol.center = new Vector3(0, 0.5f, 0);
        outerCol.height = 0.1f;
        outerCol.radius = 0.1f;
        outerCol.center = new Vector3(0, 0.5f, 0);
        rb.AddForce(0, -200, 0);
    }
}
