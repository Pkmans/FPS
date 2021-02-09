using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orangeBoi : Enemy
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Die() {
        if (died) return;
        died = true;
        
        Instantiate(deathParticle, transform.position, deathParticle.transform.rotation);

        RagDoll ragdoll = GetComponent<RagDoll>();
            if (ragdoll)
                ragdoll.ToggleRagdoll(true);

            EnemyAI ai = GetComponent<EnemyAI>();
            if (ai)
                ai.DisableAI();


        anim.enabled = false;
        GameManager.instance.StartCoroutine("deathCrosshair");
    }
}
