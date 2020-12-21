using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float speed = 0f;
    public float time = 4f;

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
        } 
        
        
        Destroy(gameObject);
    }
}
