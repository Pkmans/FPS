using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float time = 4f;
    public GameObject impactEffect;
    public LayerMask whatIsHittable;

    [HideInInspector]
    //variables passed in from gun
    public int damage;
    private bool dmgAlrdyDealt;

    private Rigidbody rb;

    //position of projectile in previous frame
    private Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Destroy(gameObject, time);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // void LateUpdate() {
    //     RaycastHit hit;
    //     if (Physics.Raycast(prevPos, transform.position, out hit, 1f, whatIsHittable)) {
    //         DoDamage(hit);
    //     }

    //     prevPos = transform.position;
    // }





    void DoDamage(RaycastHit hit) {
        // if (dmgAlrdyDealt) return;
        // dmgAlrdyDealt = true;

        print("do dmg called");

        if (hit.collider.gameObject.layer == 11)  ///enemy layer
        {  
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy) enemy.TakeDamage(damage);

            ///NEED TO ABSTRACT ENEMY HEALTH SCRIPTS
            // EnemyHP enemyHP = col.gameObject.GetComponent<EnemyHP>();        
            // if (enemyHP) enemyHP.TakeDamage(damage);
        } 
        
        if (hit.collider.gameObject.name == "Player") {
            PlayerHealth playerHp = hit.collider.gameObject.GetComponent<PlayerHealth>();
            playerHp.TakeDamage(damage);
        }
        
        //faces perpendicular to contact surface
        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.LookRotation(hit.normal, Vector3.up));

        Destroy(gameObject);
    }

    void OnCollisionEnter (Collision col) {
        if (dmgAlrdyDealt) return;

        if (col.gameObject.layer == 11)  ///enemy layer
        {  
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            if (enemy) enemy.TakeDamage(damage);

            ///NEED TO ABSTRACT ENEMY HEALTH SCRIPTS
            // EnemyHP enemyHP = col.gameObject.GetComponent<EnemyHP>();        
            // if (enemyHP) enemyHP.TakeDamage(damage);
        } 
        
        if (col.gameObject.name == "Player") {
            PlayerHealth playerHp = col.gameObject.GetComponent<PlayerHealth>();
            playerHp.TakeDamage(damage);
        }
        
        dmgAlrdyDealt = true;
        
        //faces perpendicular to contact surface
        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.LookRotation(col.contacts[0].normal, Vector3.up));

        Destroy(gameObject);
    }
}
