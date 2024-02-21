using Unity.VisualScripting;
using UnityEngine;

public class WallBuy : Interactable
{
    public GameObject weaponPrefab;
    private PlayerWeapon playerWeapon;
    private WeaponSwitching weaponSwitcher;
    private PlayerWeapon prefabWeapon;
    public int weaponCost, replenshCost;
    void Start()
    {
        prefabWeapon = weaponPrefab.GetComponent<PlayerWeapon>();
        promptMessage = "Refill Ammo " + replenshCost.ToString();
    }
    protected override void Interact()
    {
        weaponSwitcher = nearestPlayer.GetComponentInChildren<WeaponSwitching>();
        playerWeapon = weaponSwitcher.playerWeapon;
        if (playerWeapon.tag == weaponPrefab.tag)
        {
            if(nearestPlayer.Points >= replenshCost)
            {
                playerWeapon.ReplenshAmmo(replenshCost);
            }
        }
        else if(nearestPlayer.Points >= weaponCost)
        {
            //Destroys Player Current One then Creates new Weapon
            if (weaponSwitcher.transform.childCount >= weaponSwitcher.maxWeaponCount)
            {
                Destroy(playerWeapon.GameObject());
            }
            weaponPrefab.GetComponentInChildren<PlayerWeapon>().Player = nearestPlayer.gameObject;
            GameObject newWeapon = Instantiate(weaponPrefab, prefabWeapon.hipfirePosition, prefabWeapon.hipfireRotation);
            newWeapon.transform.parent = weaponSwitcher.transform;
            newWeapon.transform.localPosition = prefabWeapon.hipfirePosition;
            newWeapon.transform.localRotation = prefabWeapon.hipfireRotation;
            weaponSwitcher.WeaponNext();
        }
    }
}
