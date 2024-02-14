using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

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
                weapon.gameObject.SetActive(true);
                PlayerWeapon playerWeapon = weapon.gameObject.GetComponent<PlayerWeapon>();
                UIManager playerUI = playerWeapon.Player.GetComponentInChildren<UIManager>();
                playerUI.UpdateAmmoUI(playerWeapon.ammoLeft.ToString());
                playerUI.UpdateAmmoReserveUI(playerWeapon.ammoReserve.ToString());
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
