using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable
{
    private GameObject[] allPlayers;
    private GameObject nearestPlayer;
    private PlayerPoints nearestPlayerPoints;
    private float distance;
    private float nearestDistance = 10000;
    private bool doorOpen;
    [SerializeField] private int doorCost;
    [SerializeField] private GameObject[] spawnBarriers;

    // Start is called before the first frame update
    void Start()
    {
        promptMessage = "Door Cost " + doorCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        if (!doorOpen)
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
            if (nearestPlayer != null)
            {
                nearestPlayerPoints = nearestPlayer.GetComponentInChildren<PlayerPoints>();
                if (nearestPlayerPoints.Points >= doorCost)
                {
                    nearestPlayerPoints.Points -= doorCost;
                    nearestPlayer.GetComponentInChildren<UIManager>().UpdatePointsUI();
                    doorOpen = true;
                    GetComponent<Animator>().SetBool("IsOpen", doorOpen);
                    GetComponent<NavMeshObstacle>().enabled = false;
                    GetComponent<BoxCollider>().enabled = false;
                    for(int i = 0; i < spawnBarriers.Length; i++)
                    {
                        spawnBarriers[i].SetActive(true);
                    }
                }
            }
        }
    }
}
