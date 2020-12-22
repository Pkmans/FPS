using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float bulletSpeed;
    public float fireRate;
    public int damage;
    public int numBullets = 1;

    public GameObject bullet;
    public Transform firePoint;

    private Transform cam;
    private Camera camComponent;

    //particles
    public ParticleSystem muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("playerCamera").transform;
        camComponent = cam.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Fire();
    }

    void Fire() {
        Ray ray = camComponent.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //update targetpoint if something is closer
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(1000);


        //instantiate bullet
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        newBullet.GetComponent<Bullet>().damage = damage;

        //set bullet velocity
        Vector3 bulletDir;
        bulletDir = targetPoint - firePoint.position;
        newBullet.GetComponent<Rigidbody>().velocity = bulletDir.normalized * bulletSpeed;

        //particles and effects
        muzzleFlash.Play();
    }
}
