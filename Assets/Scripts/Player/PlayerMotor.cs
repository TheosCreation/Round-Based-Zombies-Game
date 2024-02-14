using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;

    public CharacterController controller;
    public PlayerWeapon playerWeapon;
    private Vector3 playerVelocity;
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
        playerStateMachine = GetComponent<PlayerStateMachine>();
        controller = GetComponent<CharacterController>();
        playerHeight = controller.height;
    }

    // Update is called once per frame
    void Update()
    {
        playerStateMachine.isGrounded = controller.isGrounded;
        if(lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if(playerStateMachine.isCrouching)
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
        if(playerStateMachine.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if(playerStateMachine.isGrounded) 
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void StartCrouch()
    {
        if(!playerStateMachine.isSprinting)
        {
            crouchTimer = 0;
            lerpCrouch = true;
            speed = playerWalkSpeed/2;
            playerStateMachine.isCrouching = true;
        }
    }

    public void EndCrouch()
    {
        if(playerStateMachine.isCrouching)
            speed = playerWalkSpeed;
            crouchTimer = 0;
            lerpCrouch = true;
        playerStateMachine.isCrouching = false;
    }

    public void StartSprint()
    {
        if(!playerStateMachine.isCrouching && !playerStateMachine.isAiming)
        {
            playerWeapon.ReloadCancel();
            playerStateMachine.isSprinting = true;
            speed = playerWalkSpeed * sprintSpeedMultiplier;
        }
    }

    public void CancelSprint()
    {
        playerStateMachine.cancelSprint = true;
        speed = playerWalkSpeed;
        playerStateMachine.isSprinting = false;
    }
    
    public void EndSprint()
    {
        if(playerStateMachine.isSprinting && !playerStateMachine.isCrouching && !playerStateMachine.isAiming && !playerStateMachine.cancelSprint)
            speed = playerWalkSpeed;
        playerStateMachine.isSprinting = false;
        playerStateMachine.cancelReload = false;
    }

    public void PerformSprint()
    {

    }
}
