using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public float bulletSpeed = 30f;
    public float fireRate;
    public int damage;
    public int numBullets = 1;

    public GameObject bullet;
    public Transform firePoint;

    private Transform player;

    public bool inAttackState;
    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (inAttackState && canAttack)
            Fire();
    }

    void Fire() {

        GameObject newBullet = Instantiate(bullet, firePoint.position, firePoint.localRotation);

        //set bullet velocity and direction
        Vector3 bulletDir = (player.position - transform.position).normalized;
        newBullet.GetComponent<Rigidbody>().velocity = bulletDir * bulletSpeed;
        newBullet.GetComponent<Bullet>().damage = damage;

        //fire rate logic
        canAttack = false;
        Invoke("ResetAttack", fireRate);

    }

    void ResetAttack() {
        canAttack = true;
    }

}
