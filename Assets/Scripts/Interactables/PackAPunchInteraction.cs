using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackAPunchInteraction : Interactable
{
    private GameObject[] allPlayers;
    private GameObject nearestPlayer;
    private float distance;
    private float nearestDistance = 10000;
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
            nearestPlayer.GetComponentInChildren<PackAPunch>().PAP();
        }
    }
}
