using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public CharacterController controller;
    public PlayerWeapon playerWeapon;
    private Vector3 playerVelocity;
    public bool isGrounded;
    public bool isSprinting, isCrouching, sprintCancel;
    public bool isAiming;
    private bool lerpCrouch;
    private float crouchTimer;
    private float playerHeight;
    public float playerWalkSpeed = 4f;
    public float playerSprintSpeed = 20f;
    public float speed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float sprintSpeedMultiplier = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerHeight = controller.height;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if(lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if(isCrouching)
            {
                controller.height = Mathf.Lerp(controller.height, playerHeight/1.3f, p);
            }
            else 
            {
                controller.height = Mathf.Lerp(controller.height, playerHeight, p);
            }
            
            if(p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }
    
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2;
        }
        controller.Move(playerVelocity * Time.deltaTime);
        //Debug.Log(playerVelocity.y);
    }

    public void Jump()
    {
        if(isGrounded) 
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void StartCrouch()
    {
        if(!isSprinting)
        {
            crouchTimer = 0;
            lerpCrouch = true;
            speed = playerWalkSpeed/2;
            isCrouching = true;
        }
    }

    public void EndCrouch()
    {
        if(isCrouching)
            speed = playerWalkSpeed;
            crouchTimer = 0;
            lerpCrouch = true;
        isCrouching = false;
    }

    public void StartSprint()
    {
        if(!isCrouching && !isAiming)
        {
            playerWeapon.ReloadCancel();
            isSprinting = true;
            speed = playerWalkSpeed * sprintSpeedMultiplier;
        }
    }

    public void CancelSprint()
    {
        sprintCancel = true;
        speed = playerWalkSpeed;
        isSprinting = false;
    }
    
    public void EndSprint()
    {
        if(isSprinting && !isCrouching && !isAiming && !sprintCancel)
            speed = playerWalkSpeed;
        isSprinting = false;
        sprintCancel = false;
    }

    public void PerformSprint()
    {

    }
}
