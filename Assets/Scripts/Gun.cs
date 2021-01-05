using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    public float bulletSpeed;
    public float fireRate;
    public int damage;
    public int numBullets = 1;

    public float recoil_Strength;
    private float recoilTime;
    private Vector3 initialPosition;

    //ammo system
    public int maxAmmo;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool reloading;
    public TextMeshProUGUI ammo;

    public GameObject bullet;
    public Transform firePoint;

    private Transform cam;
    private Camera camComponent;

    //particles and effects
    public ParticleSystem muzzleFlash;
    public Vector3 upRecoil;
    // [HideInInspector]
    public Quaternion originalRotation;

    

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("playerCamera").transform;
        camComponent = cam.GetComponent<Camera>();

        currentAmmo = maxAmmo;
        
    }

    void OnEnable() {
        ammo.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        ammo.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();

        if (Input.GetMouseButtonDown(0)) {
            Fire();
            recoilTime = 0.1f;
        }
            
        Recoiling();
    }

    void Fire() {
        if (reloading) return;

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
        currentAmmo--;

        if (currentAmmo <= 0)
            StartCoroutine(Reload());
    }

    IEnumerator Reload() {
        reloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        reloading = false;
    }

    void Recoiling() {
        if (recoilTime > 0) {
            Quaternion maxRecoil = Quaternion.Euler(0, recoil_Strength, 0);
            Vector3 recoilPosition = new Vector3(0, -0.2f, 0.3f) + initialPosition;

            transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPosition, Time.deltaTime * 20);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * 20);
            recoilTime -= Time.deltaTime;
        }
        else {
            recoilTime = 0;
            Quaternion minRecoil = Quaternion.Euler(0, 0, 0);
            
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 10);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, minRecoil, Time.deltaTime * 10);
        } 
    }

}
