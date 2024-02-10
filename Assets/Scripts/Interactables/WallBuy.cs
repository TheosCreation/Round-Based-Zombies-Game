using UnityEngine;

public class WallBuy : Interactable
{
    private GameObject[] allPlayers;
    private GameObject nearestPlayer;
    [SerializeField] private GameObject weaponPrefab;
    private float distance;
    private float nearestDistance = 10000;
    private string weaponCostString, replenshCostString;
    void Start()
    {
        weaponCostString = weaponPrefab.GetComponent<PlayerWeapon>().weaponCost.ToString();
        replenshCostString = weaponPrefab.GetComponent<PlayerWeapon>().replenshCost.ToString();
        promptMessage = "Refull Ammo " + replenshCostString;
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
        //attempts to pack a punch
        if(nearestPlayer != null)
        {
            nearestPlayer.GetComponentInChildren<PlayerWeapon>().ReplenshAmmo();
        }
    }
}
