using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Movement Variables")]
    public bool isGrounded, isSprinting, isCrouching;
    public bool cancelSprint;
    [Header("Weapon Variables")]
    public bool canMelee, canShoot;
    public bool isShooting, isMeleeing, isReloading, isAiming, inAimingMode;
    public bool cancelReload;
}
