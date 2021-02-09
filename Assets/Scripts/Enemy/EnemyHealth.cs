using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 1;
    public GameObject blood;
    public GameObject damageCrosshair, deathCrosshair;

    private Animator anim;
    private Transform bloodSplatterPos;

    [HideInInspector]
    public bool madeFromSpawner;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        bloodSplatterPos = gameObject.transform.Find("bloodSplatterPos");
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
            
        GameManager.instance.StartCoroutine("damageCrosshair");
    }


    void Die() {
        Instantiate(blood, bloodSplatterPos.position, blood.transform.rotation);

        RagDoll ragdoll = GetComponent<RagDoll>();
            if (ragdoll)
                ragdoll.ToggleRagdoll(true);

            EnemyAI ai = GetComponent<EnemyAI>();
            if (ai)
                ai.DisableAI();


        if (madeFromSpawner)
            GameObject.FindWithTag("Spawner").GetComponent<Spawner>().enemyCount--;
            
        anim.enabled = false;

        GameManager.instance.StartCoroutine("deathCrosshair");
    }


}
