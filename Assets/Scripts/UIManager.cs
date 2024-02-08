using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FirstRoundDisplay;
    [SerializeField] private TextMeshProUGUI RoundText;
    private float displayTimer = 0f;
    private float timeToDisplay = 3.5f;
    private bool displayActive = false;
    private bool displayedRound1 = false;
    void Start()
    {
        FirstRoundDisplay.text = "";
        RoundText.text = "";
        FirstRoundDisplay.enableWordWrapping = true;
    }
    void Update()
    {
        if(displayActive)
        {
            displayTimer += Time.deltaTime;
        }
        if(displayTimer >= timeToDisplay)
        {
            FirstRoundDisplay.text = "";
            displayActive = false;
            displayTimer = 0;
            if(!displayedRound1)
            {
                UpdateRoundUI(1);
                displayedRound1 = true;
            }
        }
    }

    public void FirstRoundAlert()
    {
        displayActive = true;
        FirstRoundDisplay.text = "Round 1";
    }

    public void UpdateRoundUI(int Round)
    {
        displayActive = true;
        RoundText.text = Round.ToString();
    }
    
}
