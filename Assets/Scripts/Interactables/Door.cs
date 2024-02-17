using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable
{
    private PlayerPoints nearestPlayerPoints;
    private bool doorOpen;
    [SerializeField] private int doorCost;
    [SerializeField] private GameObject[] spawnBarriers;

    void Start()
    {
        promptMessage = "Door Cost " + doorCost.ToString();
    }

    protected override void Interact()
    {
        if (!doorOpen)
        {
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
