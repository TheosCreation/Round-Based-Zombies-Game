using UnityEngine;

public class WeaponActions : MonoBehaviour
{
    private WeaponSwitching weaponSwitcher;

    private void Awake()
    {
        weaponSwitcher = GetComponentInChildren<WeaponSwitching>();
    }
    public void StartShot()
    {
        weaponSwitcher.playerWeapon.StartShot();
    }
    public void EndShot()
    {
        weaponSwitcher.playerWeapon.EndShot();
    }
    public void StartAim()
    {
        weaponSwitcher.playerWeapon.StartAim();
    }
    public void EndAim()
    {
        weaponSwitcher.playerWeapon.EndAim();
    }
    public void Reload()
    {
        weaponSwitcher.playerWeapon.Reload();
    }
}
