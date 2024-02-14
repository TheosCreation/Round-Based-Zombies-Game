using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackAPunch : MonoBehaviour
{
    private PlayerPoints playerPoints;
    private UIManager playerUI;

    private int papCost = 5000;
    public int papTier = 0;
    public int maxPapTier = 3;
    public float papDamageChange;
    public float papAmmoChange;

    void Awake()
    {
        playerPoints = GetComponentInParent<PlayerPoints>();
        playerUI = GetComponentInParent<UIManager>();
    }

    public void PAP()
    {
        if(papTier < maxPapTier && playerPoints.Points >= papCost)
        {
            papTier++;
            playerPoints.Points-= papCost;
            playerUI.UpdatePointsUI();
            GetComponent<PlayerWeapon>().UpdateWeaponStats();
        }
        Debug.Log("Pap level" + papTier);
    }
}
