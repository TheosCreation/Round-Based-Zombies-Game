using System;
using Unity.VisualScripting;
using UnityEngine;

public class WallBuy : Interactable
{
    private GameObject[] allPlayers;
    public GameObject nearestPlayer;
    public GameObject weaponPrefab;
    private PlayerWeapon weapon;
    private PlayerMotor motor;
    public string weaponCostString, replenshCostString;
    void Start()
    {
        weapon = weaponPrefab.GetComponent<PlayerWeapon>();
        weaponCostString = weapon.weaponCost.ToString();
        replenshCostString = weapon.replenshCost.ToString();
        promptMessage = "Refill Ammo " + replenshCostString;
    }
    protected override void Interact()
    {
        weapon = nearestPlayer.GetComponentInChildren<WeaponSwitching>().playerWeapon;
        if (weapon.tag == weaponPrefab.tag)
        {
            if(nearestPlayer.GetComponent<PlayerPoints>().Points >= weapon.replenshCost)
            {
                weapon.ReplenshAmmo();
            }
        }
        else if(nearestPlayer.GetComponentInChildren<PlayerPoints>().Points >= weapon.weaponCost)
        {
            //Creates new Weapon and Destroys Player Current One
            weaponPrefab.GetComponentInChildren<PlayerWeapon>().Player = nearestPlayer;
            GameObject newWeapon = Instantiate(weaponPrefab, weapon.hipfirePosition, weapon.hipfireRotation);
            newWeapon.transform.SetParent(nearestPlayer.GetComponentInChildren<PlayerWeapon>().transform);
            motor = nearestPlayer.GetComponent<PlayerMotor>();
            motor.playerWeapon = newWeapon.GetComponent<PlayerWeapon>();   
            Destroy(nearestPlayer.GetComponentInChildren<PlayerWeapon>().GameObject());
        }
    }
}
