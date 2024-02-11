using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackAPunch : MonoBehaviour
{
    private int papCost = 5000;
    public int papTier = 0;
    public int maxPapTier = 3;
    public float papDamageChange;
    public float papAmmoChange;
    private PlayerPoints playerPoints;
    [SerializeField] private UIManager UI;

    void Start()
    {
        playerPoints = GetComponentInParent<PlayerPoints>();
    }

    public void PAP()
    {
        if(papTier < maxPapTier && playerPoints.Points >= papCost)
        {
            papTier++;
            playerPoints.Points-= papCost;
            UI.UpdatePointsUI();
            GetComponent<PlayerWeapon>().UpdateWeaponStats();
        }
        Debug.Log("Pap level" + papTier);
    }
}
