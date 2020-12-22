using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 1;
    private Animator anim;

    public GameObject blood;
    private Transform bloodSplatterPos;

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

    void OnCollisionEnter (Collision col) {
       
    }

    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) 
            Die();

        Instantiate(blood, bloodSplatterPos.position, blood.transform.rotation);
    }

    void Die() {
        RagDoll ragdoll = GetComponent<RagDoll>();
            if (ragdoll)
                ragdoll.ToggleRagdoll(true);

            EnemyAI ai = GetComponent<EnemyAI>();
            if (ai)
                ai.DisableAI();

        anim.enabled = false;
    }


}
