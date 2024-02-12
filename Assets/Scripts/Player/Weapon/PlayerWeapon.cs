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
    private PackAPunch packAPunch;

    
    [Header("Weapon Values")]
    public Vector3 aimingPosition;
    public Quaternion aimingRotation;
    public Vector3 hipfirePosition;
    public Quaternion hipfireRotation;
    [SerializeField] private PlayerMelee knife;
    [SerializeField] private Transform gunBarrel;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private float bulletRange;
    [SerializeField] private float fireRate, reloadTime;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private int magSize;
    [SerializeField] private int magsToStart;
    public int weaponCost, replenshCost;
    public int ammoReserve;
    public int ammoLeft;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float headshotMultiplier;
    private bool isShooting, readyToShoot, reloading, isAiming, inAimingMode;
    private int currentPapTier;
    private RaycastHit hit;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject MuzzleFlashPrefab;
    private float bulletHoleLifeSpan = 5.0f;

    private Animator animator;
    [SerializeField] private Animator armanimator;

     [Header("Audio")]
    [SerializeField] private AudioSource audioInCamera;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip[] fireSounds;


    void Awake()
    {
        animator = GetComponent<Animator>();
        packAPunch = GetComponent<PackAPunch>();
        ammoLeft = magSize;
        ammoReserve = magSize * magsToStart;
        readyToShoot = true;
    }
    void Start()
    {
        UI.UpdateAmmoUI(ammoLeft.ToString());
        UI.UpdateAmmoReserveUI(ammoReserve.ToString());
    }

    void Update()
    {
        if(isShooting && readyToShoot && !reloading && ammoLeft > 0 && !knife.isMeleeing)
        {
            PerformShot();
        } else if(isShooting && ammoLeft <= 0 && ammoReserve > 0 && !knife.isMeleeing) 
        {
            Reload();
        }

        if (isAiming && !reloading && !inAimingMode && !knife.isMeleeing)
        {
            playerPoints.GetComponent<PlayerMotor>().isAiming = true;
            inAimingMode = true;
            animator.SetBool("isAiming", true);
            UI.ToggleCrosshair();
            UI.ToggleRedDot();
        }
        if(knife.isMeleeing)
        {
            EndAim();
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

    public void UpdateWeaponStats()
    {
        //if increase in PAP Tier apply weapon changes
        if(packAPunch.papTier > currentPapTier)
        {
            currentPapTier = packAPunch.papTier;
            bulletDamage *= packAPunch.papDamageChange;
            magSize = Mathf.RoundToInt(magSize * packAPunch.papAmmoChange);
            ammoLeft = magSize;
            ammoReserve = magSize * magsToStart;
            UI.UpdateAmmoUI(ammoLeft.ToString());
            UI.UpdateAmmoReserveUI(ammoReserve.ToString());
        }
    }

    public void PerformShot()
    {
        readyToShoot = false;
        Vector3 direction = cam.transform.forward;

        GameObject muzzleFlash = Instantiate(MuzzleFlashPrefab, gunBarrel.position, Quaternion.identity);
        muzzleFlash.transform.rotation = gunBarrel.rotation;
        Destroy(muzzleFlash, 0.02f);
        
        PlayWeaponFireSound();


        if (Physics.Raycast(cam.transform.position, direction, out hit, bulletRange))
        {
            //Debug.Log(hit.collider.gameObject.name);
            TrailRenderer trail = Instantiate(bulletTrail, gunBarrel.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
            ZombieHealth target = hit.transform.GetComponent<ZombieHealth>();
            if(target != null)
            {
                if (hit.collider.gameObject.tag == "Head")
                {
                    if (target.health - (bulletDamage * headshotMultiplier) <= 0)
                    {
                        playerPoints.Points += 90;
                    }
                    target.TakeDamage(bulletDamage * headshotMultiplier);
                }
                if (hit.collider.gameObject.tag == "Torso")
                {
                    if (target.health - (bulletDamage) <= 0)
                    {
                        playerPoints.Points += 40;
                    }
                    target.TakeDamage(bulletDamage);
                }
                if (hit.collider.gameObject.tag == "Regular")
                {
                    if (target.health - (bulletDamage) <= 0)
                    {
                        playerPoints.Points += 30;
                    }
                    target.TakeDamage(bulletDamage);
                }
                //plus 10 for hit
                playerPoints.Points += 10;
                UI.UpdatePointsUI();
                //addes blood particles
                ParticleSystem impactBloodEffectPS = Instantiate(impactBloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactBloodEffectPS, 1);
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
        UI.UpdateAmmoUI(ammoLeft.ToString());

        if (ammoLeft >= 0)
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
        isAiming = true;
    }

    public void EndAim()
    {
        if(isAiming)
        {
            inAimingMode = false;
            animator.SetBool("isAiming", false);
            UI.ToggleCrosshair();
            UI.ToggleRedDot();
            isAiming = false;
            playerPoints.GetComponent<PlayerMotor>().isAiming = false;
        }
    }

    public void Reload()
    {
        if(!reloading && ammoLeft < magSize && !isAiming && ammoReserve > 0)
        {
            reloading = true;
            knife.canMelee = false;
            audioInCamera.PlayOneShot(reloadSound);
            armanimator.SetBool("isReloading", true);
            animator.SetBool("isReloading", true);
            Invoke("ReloadFinish", reloadTime);
        }
    }

    public void ReloadFinish()
    {
        if (ammoReserve < magSize - ammoLeft)
        {
            ammoLeft = ammoLeft + ammoReserve;
            ammoReserve = 0;
        }
        else
        {
            ammoReserve -= (magSize - ammoLeft);
            ammoLeft = magSize;
        }

        armanimator.SetBool("isReloading", false);
        animator.SetBool("isReloading", false);

        UI.UpdateAmmoUI(ammoLeft.ToString());
        UI.UpdateAmmoReserveUI(ammoReserve.ToString());
        reloading = false;
        knife.canMelee = true;
    }

    public void ReplenshAmmo()
    {
        if(playerPoints.Points > replenshCost && ammoReserve != magSize * magsToStart)
        {
            playerPoints.Points -= replenshCost;
            ammoLeft = magSize;
            ammoReserve = magSize * magsToStart;
            UI.UpdatePointsUI();
            UI.UpdateAmmoUI(ammoLeft.ToString());
            UI.UpdateAmmoReserveUI(ammoReserve.ToString());
        }
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
