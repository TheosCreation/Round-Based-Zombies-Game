using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor;
    private PlayerLook look;
    private PlayerWeapon playerWeapon;
    private PlayerMelee playerMelee;
    // Start is called before the first frame update
    void Awake()
    {
        //locks the cursor so you dont see it maybe unlock when open inventory
        Cursor.lockState = CursorLockMode.Locked;

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        playerWeapon = GetComponentInChildren<PlayerWeapon>();
        playerMelee = GetComponentInChildren<PlayerMelee>();

        //Player Motor
        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Crouch.started += ctx => motor.StartCrouch();
        onFoot.Crouch.canceled += ctx => motor.EndCrouch();
        onFoot.Sprint.started += ctx => motor.StartSprint();
        onFoot.Sprint.canceled += ctx => motor.EndSprint();

        //Player Melee
        onFoot.Melee.performed += ctx => playerMelee.Melee();
        //Player Weapon
        onFoot.Shoot.started += ctx => playerWeapon.StartShot();
        onFoot.Shoot.canceled += ctx => playerWeapon.EndShot();

        onFoot.Aim.started += ctx => playerWeapon.StartAim();
        onFoot.Aim.canceled += ctx => playerWeapon.EndAim();

        onFoot.Reload.performed += ctx => playerWeapon.Reload();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
