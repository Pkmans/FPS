using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBasic : MonoBehaviour
{
    private StabilizeEnemy stabilizeScript;

    public float moveSpeed;
    public float maxTargetDistance;
    
    [Header("Attack")]
    public float bulletSpeed = 30f;
    public float fireRate;
    public int damage;

    public GameObject bullet;
    public Transform firePoint;

    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        stabilizeScript = GetComponent<StabilizeEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canAttack) 
            Attack();

        
    }

    void Attack() {
        bool inRange = DistanceFromPlayer() < maxTargetDistance;
        if (!inRange) return;

        GameObject newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation);

        //set bullet velocity and direction
        Transform player = PlayerMovement.instance.gameObject.transform;
        Vector3 bulletDir = (player.position - stabilizeScript.bodyRb.position).normalized;
        newBullet.GetComponent<Rigidbody>().velocity = bulletDir * bulletSpeed;

        newBullet.GetComponent<Bullet>().damage = damage;

        StartCoroutine(PutAttackOnCooldown());
    }

    float DistanceFromPlayer() {
        Transform player = PlayerMovement.instance.gameObject.transform;
        return (player.position - stabilizeScript.bodyRb.position).magnitude;
    }

    IEnumerator PutAttackOnCooldown() {
        canAttack = false;
        yield return new WaitForSeconds(fireRate);
        canAttack = true;
    }

}
