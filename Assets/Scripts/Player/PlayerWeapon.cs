using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    //rework
    
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
    public ParticleSystem impactBloodEffect;
    public Camera cam;
    
    [Header("Weapon Values")]
    [SerializeField] private Transform gunBarrel;
    [SerializeField] private Vector3 bulletSpread = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private float bulletRange;
    [SerializeField] private float fireRate, reloadTime;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private int magSize;
    [SerializeField] private float bulletDamage;

    private int ammoLeft;
    private bool isShooting, readyToShoot, reloading;
    private RaycastHit hit;
    void Awake()
    {
        ammoLeft = magSize;
        readyToShoot = true;
        gunBarrel = GetComponent<Transform>();
    }

    void Update()
    {
        if(isShooting && readyToShoot && !reloading && ammoLeft > 0)
        {
            PerformShot();
        }
    }

    public void StartShot()
    {
        isShooting = true;
    }

    public void EndShot()
    {
        isShooting = false;
    }

    public void PerformShot()
    {
        readyToShoot = false;
        Vector3 direction = cam.transform.forward;

        muzzleFlash.Play();
        
        if(Physics.Raycast(cam.transform.position, direction, out hit, bulletRange))
        {
            //Debug.Log(hit.collider.gameObject.name);
            TrailRenderer trail = Instantiate(bulletTrail, gunBarrel.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
            ZombieHealth target = hit.transform.GetComponent<ZombieHealth>();
            if(target != null)
            {
                target.TakeDamage(bulletDamage);
                ParticleSystem impactBloodEffectPS = Instantiate(impactBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactBloodEffectPS, 1);
            }
            else{
                ParticleSystem impactEffectPS = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactEffectPS, 1);
            }
        }

        ammoLeft--;

        if(ammoLeft >= 0)
        {
            Invoke("ResetShot", fireRate);
            
            if(!isAutomatic)
            {
                EndShot();
            }
        }
    }

    public void ResetShot()
    {
        readyToShoot = true;
    }

    public void Reload()
    {
        reloading = true;
        Invoke("ReloadFinish", reloadTime);
    }

    public void ReloadFinish()
    {
        ammoLeft = magSize;
        reloading = false;
    }
    private Vector3 GetDirection()
    {
        Vector3 direction = gunBarrel.transform.forward;

        direction += new Vector3(
            Random.Range(-bulletSpread.x, bulletSpread.x),
            Random.Range(-bulletSpread.y, bulletSpread.y),
            Random.Range(-bulletSpread.z, bulletSpread.z)
        );

        direction.Normalize();
        return direction;
    }
    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
    {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;

        while(time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        //Animator.SetBool("IsShooting", false);
        Trail.transform.position = Hit.point;

        Destroy(Trail.gameObject, Trail.time);
    }

}
