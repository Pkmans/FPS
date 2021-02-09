using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    
    public GameObject deathParticle; 
    public Transform deathParticlePos;

    protected bool died;

    // Start is called before the first frame update
    public void Start()
    {
        deathParticlePos = gameObject.transform.Find("bloodSplatterPos");
    }

    public void TakeDamage(float damage){
        health -= damage;

        if (health <= 0) {
            Die();
        }

        GameManager.instance.StartCoroutine("damageCrosshair");
    }

    public virtual void Die() {
        if (died) return;
        died = true;
        
        Instantiate(deathParticle, transform.position, deathParticle.transform.rotation);

        GameManager.instance.StartCoroutine("deathCrosshair");
    }
}
