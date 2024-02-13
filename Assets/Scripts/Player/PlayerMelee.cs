using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class PlayerMelee : MonoBehaviour
{
    public float meleeDamage;
    public float meleeDistance;
    public float meleeCooldown;
    public float meleeDuration;
    public bool canMelee = true;
    public bool isMeleeing;

    private Camera cam;
    private InputManager inputManager;
    [SerializeField] private UIManager UI;
    [SerializeField] private PlayerPoints playerPoints;
    [SerializeField] private GameObject playerWeapon;
    private float timeSinceLastMelee;
    private Animator animator;
    [SerializeField] private Animator armanimator;

    void Start()
    {
        cam = GetComponentInParent<PlayerLook>().cam;
        inputManager = GetComponentInParent<InputManager>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!canMelee)
        {
            timeSinceLastMelee += Time.deltaTime;
        }
        if (timeSinceLastMelee >= meleeCooldown)
        {
            canMelee = true;
            timeSinceLastMelee = 0;
        }

        if (timeSinceLastMelee >= meleeDuration)
        {
            isMeleeing = false;
            animator.SetBool("isMelee", isMeleeing);
            armanimator.SetBool("isMelee", isMeleeing);
            playerWeapon.GetComponentInChildren<MeshRenderer>().enabled = true;
        }
    }

    public void Melee()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if(canMelee)
        {
            playerPoints.GetComponent<PlayerMotor>().CancelSprint();
            playerWeapon.GetComponentInChildren<MeshRenderer>().enabled = false;
            isMeleeing = true;
            animator.SetBool("isMelee", isMeleeing);
            armanimator.SetBool("isMelee", isMeleeing);
            canMelee = false;
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, meleeDistance))
            {
                ZombieHealth target = hitInfo.transform.GetComponent<ZombieHealth>();
                if (target != null)
                {
                    if (target.health - meleeDamage <= 0)
                    {
                        playerPoints.Points += 130;
                    }

                    target.TakeDamage(meleeDamage);
                    //plus 10 for hit
                    playerPoints.Points += 10;
                    UI.UpdatePointsUI();
                }
            }
        }
    }
}
