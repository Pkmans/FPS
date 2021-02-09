using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Spiderboi : Enemy
{
    // public GameObject[] legs;
    // public GameObject[] targets;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public override void Die() {
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

        GameManager.instance.StartCoroutine("deathCrosshair");
        Destroy(gameObject, 0.17f);
    }


    

}
