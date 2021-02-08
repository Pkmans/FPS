using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyHP : MonoBehaviour
{
    public int health = 1;
    public GameObject deathParticle;
    public GameObject damageCrosshair, deathCrosshair;

    public GameObject[] legs;

    public GameObject[] targets;

    private bool died;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) {
            Die();
        }

        ///crosshair flash
        damageCrosshair.SetActive(true);
        Invoke("turnOffDmgCrosshair", 0.15f);
    }
    
    void Die() {
        if (died) return;
        died = true;
        Instantiate(deathParticle, transform.position, deathParticle.transform.rotation);

        // //explode enemy into pieces
        // foreach (GameObject leg in legs) {
        //     leg.AddComponent<Rigidbody>();
        //     leg.transform.parent = null;

        //     leg.GetComponentInChildren<DitzelGames.FastIK.FastIKFabric>().enabled = false;
        // }

        // foreach (GameObject t in targets) {
        //     t.GetComponent<LegStepper>().enabled = false;
        // }

        // //disable movement scripts
        // GetComponent<ghastController>().enabled = false;
        // GetComponent<NavMeshAgent>().enabled = false;
        // GetComponent<EnemyAI>().enabled = false;
        // GetComponent<EnemyGun>().enabled = false;

        deathCrosshair.SetActive(true);
        Invoke("turnOffDeathCrosshair", 0.15f);

        Destroy(gameObject, 0.15f);
    }

    void turnOffDmgCrosshair() {
        damageCrosshair.SetActive(false);
    }

    void turnOffDeathCrosshair() {
        deathCrosshair.SetActive(false);
    }

    

}
