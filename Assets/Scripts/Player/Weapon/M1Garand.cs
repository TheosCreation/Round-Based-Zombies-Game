using UnityEngine;

public class M1Garand : PlayerWeapon
{

    [SerializeField] private AudioClip finishClip;
    private bool playedFinishSound;
    protected override void Update()
    {
        if (playerStateMachine.isShooting && readyToShoot && !playerStateMachine.isReloading && ammoLeft > 0 && !playerStateMachine.isMeleeing)
        {
            PerformShot();
        }
        else if (playerStateMachine.isShooting && ammoLeft <= 0 && ammoReserve > 0 && !playerStateMachine.isMeleeing)
        {
            Reload();
        }
        else if (playerStateMachine.isShooting && !playedEmptySound && ammoLeft <= 0 && ammoReserve <= 0)
        {
            weaponSource.PlayOneShot(emptySound);
            playedEmptySound = true;
            animator.SetBool("isEmpty", true);
        }
        if(ammoLeft <= 0 && playerStateMachine.isShooting && !playedFinishSound)
        {
            weaponSource.PlayOneShot(finishClip);
            playedFinishSound = true;
        }
        if (ammoLeft <= 0)
        {
            animator.SetBool("isEmpty", true);
        }
        else
        {
            animator.SetBool("isEmpty", false);
        }

        if (playerStateMachine.isAiming && !inAimingMode)
        {
            playerStateMachine.isAiming = true;
            inAimingMode = true;
            animator.SetBool("isAiming", true);
        }
        if (playerStateMachine.isMeleeing && playerStateMachine.isReloading)
        {
            ReloadCancel();
        }
    }

    protected override void ReloadFinish()
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
            animator.SetBool("isEmpty", false);
            playedFinishSound = false;

            PlayerUI.UpdateAmmoUI(ammoLeft.ToString());
            PlayerUI.UpdateAmmoReserveUI(ammoReserve.ToString());
            playerStateMachine.isReloading = false;
        }
        playerStateMachine.cancelSprint = false;
    }
}
