﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Properties")]
    public float bulletSpeed;
    public float fireRate;
    public int damage;
    public int numBullets = 1;
    public float spreadFactor = 0.1f;

    [Header("Recoil")]
    //recoil
    public float playerKnockBack;
    public float recoil_Strength;
    public float recoilHeight;
    public float recoilBack;
    public float recoilSpeed;
    private float recoilTime;
    private Vector3 initialPosition;

    [Header("Ammo")]
    //ammo system
    public int maxAmmo;
    public int clipSize;
    private int currentClip;
    private int remainingAmmo;
    public float reloadTime = 0.75f;
    private bool reloading;
    private bool ammoEmpty;
    public TextMeshProUGUI ammo;

    [Header("E.t.c.")]
    public GameObject bullet;
    public Transform firePoint;

    private Transform cam;
    private Camera camComponent;

    //particles and effects
    public ParticleSystem muzzleFlash;
    private Animator anim;
    // [HideInInspector]
    public Quaternion originalRotation;

    private bool canFire = true;
    private PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("playerCamera").transform;
        camComponent = cam.GetComponent<Camera>();
        anim = transform.parent.GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        
        
        currentClip = clipSize;
        remainingAmmo = maxAmmo;
    }

    void OnEnable() {
        ammo.text = currentClip.ToString() + " / " + remainingAmmo.ToString();
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        ammo.text = currentClip.ToString() + " / " + remainingAmmo.ToString();

        if (Input.GetMouseButton(0) && canFire && !reloading) {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && !ammoEmpty) 
            StartCoroutine(Reload());

        Recoiling();
    }

    void Fire() {
        if (reloading) return;
        if (currentClip == 0 && ammoEmpty) {
            //play empty clip sound effect
            return;
        }

        canFire = false;
        Invoke("ReadyFire", fireRate);
        recoilTime = 0.1f;

        Ray ray = camComponent.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //update targetpoint if something is closer
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(1000);

        SpawnBullet(targetPoint);

        //particles and effects
        muzzleFlash.Play();
        currentClip--;
        AudioManager.instance.Play("pistolShot");
        if (currentClip <= 0 && !ammoEmpty) StartCoroutine(Reload());

        player.KnockBack((player.transform.position - targetPoint).normalized * playerKnockBack);

    }


    ///can probably clean up this code
    void SpawnBullet(Vector3 targetPoint) {
        //instantiate bullet
        GameObject newBullet = Instantiate(bullet, firePoint.position, firePoint.localRotation);
        newBullet.GetComponent<Bullet>().damage = damage;

        //set bullet velocity
        Vector3 bulletDir;
        bulletDir = targetPoint - firePoint.position;
        newBullet.GetComponent<Rigidbody>().velocity = bulletDir.normalized * bulletSpeed;

        ///EXTRA BULLETS WITH A SPREAD
        for (int i = 0; i < numBullets - 1; i++) 
        {
            //instantiate bullet
            GameObject newBullet1 = Instantiate(bullet, firePoint.position, firePoint.localRotation);
            newBullet1.GetComponent<Bullet>().damage = damage;

            //set bullet velocity
            Vector3 bulletDir1;
            bulletDir1 = targetPoint - firePoint.position;

            Quaternion rotation = Quaternion.Euler(Random.Range(-spreadFactor, spreadFactor), Random.Range(-spreadFactor, spreadFactor), Random.Range(-spreadFactor, spreadFactor));
            Vector3 rotateVector = rotation * bulletDir1;

            newBullet1.GetComponent<Rigidbody>().velocity = rotateVector.normalized * bulletSpeed;
        }
    } 


    IEnumerator Reload() {
        //start reload anim
        reloading = true;
        anim.enabled = true;
        anim.SetBool("reloading", true);
        AudioManager.instance.Play("reload");

        yield return new WaitForSeconds(reloadTime);

        //end reload anim
        reloading = false;
        anim.enabled = false;
        anim.SetBool("reloading", false);

        //ammo reload
        int reloadAmount = clipSize - currentClip;
        
        if (remainingAmmo < reloadAmount) //if remaining ammo too little,
            reloadAmount = remainingAmmo; //only reload remaining ammo

        currentClip += reloadAmount;
        remainingAmmo -= reloadAmount;

        if (remainingAmmo == 0) ammoEmpty = true;
    }

    void Recoiling() {
        if (recoilTime > 0) {
            Quaternion maxRecoil = Quaternion.Euler(0, recoil_Strength, 0);
            Vector3 recoilPosition = new Vector3(0, -recoilBack, recoilHeight) + initialPosition;

            //smoothly knock up to recoil position
            transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPosition, Time.deltaTime * recoilSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            recoilTime -= Time.deltaTime;
        }
        else {
            recoilTime = 0;
            Quaternion minRecoil = Quaternion.Euler(0, 0, 0);
            
            //smoothly return to initial position
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * recoilSpeed/2);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, minRecoil, Time.deltaTime * recoilSpeed/2);
        } 
    }

    void ReadyFire() {
        canFire = true;
    }

}
