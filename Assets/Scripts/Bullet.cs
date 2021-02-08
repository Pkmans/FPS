using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float time = 4f;
    public GameObject impactEffect;

    [HideInInspector]
    //variables passed in from gun
    public int damage;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Destroy(gameObject, time);
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnCollisionEnter (Collision col) {
        if (col.gameObject.layer == 11)  ///enemy layer
        {  
            EnemyHealth enemyHp = col.gameObject.GetComponent<EnemyHealth>();
            if (enemyHp) enemyHp.TakeDamage(damage);
            ///NEED TO ABSTRACT ENEMY HEALTH SCRIPTS
            EnemyHP enemyHP = col.gameObject.GetComponent<EnemyHP>();        
            if (enemyHP) enemyHP.TakeDamage(damage);
        } 
        
        if (col.gameObject.name == "Player") {
            PlayerHealth playerHp = col.gameObject.GetComponent<PlayerHealth>();
            playerHp.TakeDamage(damage);
        }
        
        //faces perpendicular to contact surface
        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.LookRotation(col.contacts[0].normal, Vector3.up));


        Destroy(gameObject);
    }
}
