using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    public int maxWeaponCount = 0;
    public PlayerWeapon playerWeapon;
    public PlayerStateMachine playerStateMachine;

    private void Start()
    {
        SelectWeapon();
    }
    public void WeaponNext()
    {
        if(selectedWeapon >= transform.childCount - 1)
        {
            selectedWeapon = 0;
        }
        else
        {
            selectedWeapon++;
        }
        SelectWeapon();
    }
    public void WeaponPrevious()
    {
        if (selectedWeapon <= 0)
        {
            selectedWeapon = transform.childCount - 1;
        }
        else
        {
            selectedWeapon--;
        }
        SelectWeapon();
    }
    public void SelectWeapon()
    {
        int i = 0;
        foreach(Transform weapon in transform)
        {
            if(i == selectedWeapon)
            {
                playerWeapon.ReloadCancel();
                weapon.gameObject.SetActive(true);
                playerWeapon = weapon.gameObject.GetComponent<PlayerWeapon>();
                UIManager playerUI = playerWeapon.Player.GetComponentInChildren<UIManager>();
                playerUI.UpdateAmmoUI(playerWeapon.ammoLeft.ToString());
                playerUI.UpdateAmmoReserveUI(playerWeapon.ammoReserve.ToString());
                playerStateMachine.cancelReload = false;
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
