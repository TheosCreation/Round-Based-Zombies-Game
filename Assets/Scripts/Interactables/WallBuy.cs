using Unity.VisualScripting;
using UnityEngine;

public class WallBuy : Interactable
{
    private GameObject[] allPlayers;
    private GameObject nearestPlayer;
    [SerializeField] private GameObject weaponPrefab;
    private PlayerWeapon weapon;
    private PlayerMotor motor;
    private float distance;
    private float nearestDistance = 10000;
    private string weaponCostString, replenshCostString;
    void Start()
    {
        weapon = weaponPrefab.GetComponent<PlayerWeapon>();
        weaponCostString = weapon.weaponCost.ToString();
        replenshCostString = weapon.replenshCost.ToString();
        promptMessage = "Refill Ammo " + replenshCostString;
    }
    protected override void Interact()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < allPlayers.Length; i++)
        {
            distance = Vector3.Distance(this.transform.position, allPlayers[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestPlayer = allPlayers[i];
            }
        }
        //attempts to wall buy
        if(nearestPlayer != null)
        {
            if(nearestPlayer.GetComponentInChildren<PlayerWeapon>() == weapon)
            {
                if(nearestPlayer.GetComponent<PlayerPoints>().Points >= weapon.replenshCost)
                {
                    nearestPlayer.GetComponentInChildren<PlayerWeapon>().ReplenshAmmo();
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
}
