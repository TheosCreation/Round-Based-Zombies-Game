using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject Player;
    private PlayerStateMachine playerStateMachine;
    private UIManager PlayerUI;
    private Camera cam;
    private PlayerPoints playerPoints;
    private PlayerMotor playerMotor;

    //implement hipfire spread and use isaiming to ignore spread
    [Header("Weapon Values")]
    private PackAPunch packAPunch;
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
    [SerializeField] private int magsToStart;
    public int ammoReserve;
    public int ammoLeft;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float headshotMultiplier;
    private bool readyToShoot, inAimingMode;
    private int currentPapTier;
    private RaycastHit hit;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject MuzzleFlashPrefab;
    public ParticleSystem impactEffect;
    public ParticleSystem impactBloodEffect;
    private float bulletHoleLifeSpan = 5.0f;

    private Animator animator;

     [Header("Audio")]
    private AudioSource weaponSource;
    private AudioSource reloadSource;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptySound;
    private bool playedEmptySound;
    [SerializeField] private AudioClip[] fireSounds;


    void Awake()
    {
        playerStateMachine = Player.GetComponent<PlayerStateMachine>();
        PlayerUI = Player.GetComponentInChildren<UIManager>();
        cam = Player.GetComponentInChildren<Camera>();
        playerPoints = Player.GetComponent<PlayerPoints>();
        playerMotor = Player.GetComponent<PlayerMotor>();
        animator = GetComponent<Animator>();
        packAPunch = GetComponent<PackAPunch>();
        ammoLeft = magSize;
        ammoReserve = magSize * magsToStart;
        readyToShoot = true;
        PlayerUI.UpdateAmmoUI(ammoLeft.ToString());
        PlayerUI.UpdateAmmoReserveUI(ammoReserve.ToString());
        weaponSource = Player.GetComponent<AudioSource>();
        reloadSource = GetComponentInChildren<AudioSource>();

    }
    void Start()
    {
    }

    void Update()
    {
        if(playerStateMachine.isShooting && readyToShoot && !playerStateMachine.isReloading && ammoLeft > 0 && !playerStateMachine.isMeleeing)
        {
            PerformShot();
        } else if(playerStateMachine.isShooting && ammoLeft <= 0 && ammoReserve > 0 && !playerStateMachine.isMeleeing) 
        {
            animator.SetBool("isShooting", false);
            Reload();
        }
        else if (playerStateMachine.isShooting && !playedEmptySound)
        {
            animator.SetBool("isShooting", false);
            weaponSource.PlayOneShot(emptySound);
            playedEmptySound = true;
        }
        if(ammoLeft <= 0 && ammoReserve > 0)
        {
            animator.SetBool("isEmpty", true);
        }
        else if(ammoLeft <= 0)
        {
            animator.SetBool("isEmpty", true);
        }
        else
        {
            animator.SetBool("isEmpty", false);
        }

        if (playerStateMachine.isAiming )
        {
            playerStateMachine.isAiming = true;
            inAimingMode = true;
            animator.SetBool("isAiming", true);
            PlayerUI.ToggleCrosshair();
            PlayerUI.ToggleRedDot();
        }
        if(playerStateMachine.isMeleeing && playerStateMachine.isReloading)
        {
            ReloadCancel();
        }
    }

    public void StartShot()
    {
        playerStateMachine.isShooting = true;
    }

    public void EndShot()
    {
        playerStateMachine.isShooting = false;
        playerStateMachine.cancelSprint = false;
        animator.SetBool("isShooting", false);
        playedEmptySound = false;
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
            PlayerUI.UpdateAmmoUI(ammoLeft.ToString());
            PlayerUI.UpdateAmmoReserveUI(ammoReserve.ToString());
        }
    }

    public void PerformShot()
    {
        animator.SetBool("isShooting", true);
        readyToShoot = false;
        Vector3 direction = cam.transform.forward;

        GameObject muzzleFlash = Instantiate(MuzzleFlashPrefab, gunBarrel.position, Quaternion.identity);
        muzzleFlash.transform.rotation = gunBarrel.rotation;
        Destroy(muzzleFlash, 0.02f);
        
        PlayWeaponFireSound();
        playerMotor.CancelSprint();

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
                PlayerUI.UpdatePointsUI();
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
        PlayerUI.UpdateAmmoUI(ammoLeft.ToString());

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
        playerStateMachine.isAiming = true;
        playerMotor.CancelSprint();
    }

    public void EndAim()
    {
        if(playerStateMachine.isAiming)
        {
            inAimingMode = false;
            animator.SetBool("isAiming", false);
            PlayerUI.ToggleCrosshair();
            PlayerUI.ToggleRedDot();
            playerStateMachine.isAiming = false;
            playerStateMachine.isAiming = false;
            playerStateMachine.cancelSprint = false;
        }
    }

    public void Reload()
    {
        if(!playerStateMachine.isReloading && ammoLeft < magSize && !playerStateMachine.isAiming && ammoReserve > 0)
        {
            playerStateMachine.isReloading = true;
            reloadSource.PlayOneShot(reloadSound);
            animator.SetBool("isEmpty", false);
            animator.SetBool("isReloading", true);
            playerMotor.CancelSprint();
            Invoke("ReloadFinish", reloadTime);
        }
    }
    public void ReloadCancel()
    {
        reloadSource.Stop();
        playerStateMachine.isReloading = false;
        playerStateMachine.cancelReload = true;
        animator.SetBool("isReloading", false);
    }

    public void ReloadFinish()
    {
        if (!playerStateMachine.cancelReload)
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

            animator.SetBool("isReloading", false);

            PlayerUI.UpdateAmmoUI(ammoLeft.ToString());
            PlayerUI.UpdateAmmoReserveUI(ammoReserve.ToString());
            playerStateMachine.isReloading = false;
        }
        playerStateMachine.cancelSprint = false;
    }

    public void ReplenshAmmo(int replenshCost)
    {
        if(ammoReserve != magSize * magsToStart)
        {
            playerPoints.Points -= replenshCost;
            ammoLeft = magSize;
            ammoReserve = magSize * magsToStart;
            PlayerUI.UpdatePointsUI();
            PlayerUI.UpdateAmmoUI(ammoLeft.ToString());
            PlayerUI.UpdateAmmoReserveUI(ammoReserve.ToString());
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
        weaponSource.PlayOneShot(fire);
    }
}
