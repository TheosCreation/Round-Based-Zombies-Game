using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackAPunch : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private int papCost = 5000;
    public int papTier = 0;
    public int maxPapTier = 3;
    public float papDamageChange;
    public float papAmmoChange;

    void Awake()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
    }
    public void PAP()
    {
        if(papTier < maxPapTier && playerWeapon.Player.GetComponent<PlayerPoints>().Points >= papCost)
        {
            papTier++;
            playerWeapon.Player.GetComponent<PlayerPoints>().Points-= papCost;
            playerWeapon.Player.GetComponentInChildren<UIManager>().UpdatePointsUI();
            playerWeapon.UpdateWeaponStats();
        }
    }
}
