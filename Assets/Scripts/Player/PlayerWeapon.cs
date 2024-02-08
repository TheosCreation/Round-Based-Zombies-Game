using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    //implement hipfire spread and use isaiming to ignore spread
    public UIManager UI;
    public ParticleSystem impactEffect;
    public ParticleSystem impactBloodEffect;
    public Camera cam;
    public PlayerPoints playerPoints;
    
    [Header("Weapon Values")]
    public Vector3 aimingPosition;
    public Quaternion aimingRotation;
    public Vector3 hipfirePosition;
    public Quaternion hipfireRotation;
    [SerializeField] private Transform gunBarrel;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private float bulletRange;
    [SerializeField] private float fireRate, reloadTime;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private int magSize;
    [SerializeField] private float bulletDamage;
    public int ammoLeft;
    private bool isShooting, readyToShoot, reloading;//isAiming;
    private RaycastHit hit;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject MuzzleFlashPrefab;
    private float bulletHoleLifeSpan = 5.0f;

     [Header("Audio")]
    public AudioSource audioInCamera;
    public AudioClip[] fireSounds;


    void Awake()
    {
        ammoLeft = magSize;
        readyToShoot = true;
    }

    void Update()
    {
        if(isShooting && readyToShoot && !reloading && ammoLeft > 0)
        {
            PerformShot();
        } else if(isShooting && ammoLeft <= 0 && !readyToShoot) 
        {
            Reload();
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

        GameObject muzzleFlash = Instantiate(MuzzleFlashPrefab, gunBarrel.position, Quaternion.identity);
        muzzleFlash.transform.rotation = gunBarrel.rotation;
        Destroy(muzzleFlash, 0.02f);
        
        PlayWeaponFireSound();
        
        if(Physics.Raycast(cam.transform.position, direction, out hit, bulletRange))
        {
            //Debug.Log(hit.collider.gameObject.name);
            TrailRenderer trail = Instantiate(bulletTrail, gunBarrel.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
            ZombieHealth target = hit.transform.GetComponent<ZombieHealth>();
            if(target != null)
            {
                target.TakeDamage(bulletDamage);
                ParticleSystem impactBloodEffectPS = Instantiate(impactBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactBloodEffectPS, 1);
                //award more points for headshot
                playerPoints.Points += 10;
                UI.UpdatePointsUI(playerPoints.Points);
            }
            else{
                ParticleSystem impactEffectPS = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity);
                bulletHole.transform.LookAt(hit.point + hit.normal);
                Destroy(bulletHole, bulletHoleLifeSpan);
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
    
    public void StartAim()
    {
        //isAiming = true;
        transform.localPosition = aimingPosition;
        transform.localRotation = aimingRotation;
        UI.ToggleCrosshair();
        UI.ToggleRedDot();
        
    }

    public void EndAim()
    {
        transform.localPosition = hipfirePosition;
        transform.localRotation = hipfireRotation;
        UI.ToggleCrosshair();
        UI.ToggleRedDot();
        //isAiming = false;
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

    private void PlayWeaponFireSound()
    {
        AudioClip fire = fireSounds[Random.Range(0, fireSounds.Length)];
        audioInCamera.PlayOneShot(fire);
    }
}
