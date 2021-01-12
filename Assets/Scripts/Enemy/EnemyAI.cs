using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public LayerMask whatIsPlayer;

    //navmesh stuff
    // public Vector3 walkPoint;
    public Transform player;

    //states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private Animator anim;
    private EnemyGun enemyGun;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyGun = GetComponentInChildren<EnemyGun>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        if (playerInSightRange && !playerInAttackRange) ChasePlayer();

        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        else if (enemyGun)
            enemyGun.inAttackState = false;
    }

    void ChasePlayer() {
        agent.SetDestination(player.position);

        //look at player
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(playerPos);

        anim.SetTrigger("run");
        
    }

    void AttackPlayer() {
        
        //stop moving and play animation
        agent.SetDestination(transform.position);
        anim.SetTrigger("attack");

        //look at player
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(playerPos);

        if (enemyGun)
            enemyGun.inAttackState = true;
        
    }

    public void DisableAI() {
        agent.SetDestination(transform.position);   ///stops movement
        this.enabled = false;
    }
}
