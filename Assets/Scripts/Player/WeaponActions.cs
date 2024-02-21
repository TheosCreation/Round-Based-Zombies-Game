using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class WeaponActions : NetworkBehaviour
{
    private WeaponSwitching weaponSwitcher;

    private void Awake()
    {
        weaponSwitcher = GetComponentInChildren<WeaponSwitching>();
    }
    public void StartShot()
    {
        if (!IsOwner)
        {
            return;
        }
        weaponSwitcher.playerWeapon.StartShot();
    }
    public void EndShot()
    {
        if (!IsOwner)
        {
            return;
        }
        weaponSwitcher.playerWeapon.EndShot();
    }
    public void StartAim()
    {
        if (!IsOwner)
        {
            return;
        }
        weaponSwitcher.playerWeapon.StartAim();
    }
    public void EndAim()
    {
        if (!IsOwner)
        {
            return;
        }
        weaponSwitcher.playerWeapon.EndAim();
    }
    public void Reload()
    {
        if (!IsOwner)
        {
            return;
        }
        weaponSwitcher.playerWeapon.Reload();
    }
}
