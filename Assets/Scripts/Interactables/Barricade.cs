using UnityEngine;

public class Barricade : Interactable
{
    private int currentPlanks;
    [SerializeField] private int maxPlanks;
    public GameObject plankPrefab;
    protected override void Interact()
    {
        if(nearestPlayer != null)
        {
            //if barricade amount < max barricade amount then spawn barricade obj and award player 10 points
            if(currentPlanks < maxPlanks)
            {
                //spawn plank
                GameObject plank = Instantiate(plankPrefab, transform.position, Quaternion.identity);
                plank.transform.parent = transform;
                plank.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-25, 25));
                plank.transform.position = new Vector3(transform.position.x, Random.Range(transform.position.y-0.3f, transform.position.y + 0.3f), transform.position.z);
                
                //give player 10 points
                nearestPlayer.GetComponent<PlayerPoints>().Points += 10;
                nearestPlayer.GetComponentInChildren<UIManager>().UpdatePointsUI();
            }
            currentPlanks = transform.childCount;
        }
    }
}
