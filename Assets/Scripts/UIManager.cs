using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FirstRoundDisplay;
    [SerializeField] private TextMeshProUGUI RoundText;
    [SerializeField] private TextMeshProUGUI PointsText;
    [SerializeField] private TextMeshProUGUI AmmoLeftText;
    [SerializeField] private TextMeshProUGUI AmmoReserverText;
    [SerializeField] private GameObject Crosshair;
    [SerializeField] private GameObject RedDot;
    private float displayTimer = 0f;
    private float timeToDisplay = 3.5f;
    private bool displayActive = true;
    private bool displayedRound1 = false;
    private bool crosshairToggle  = true;
    private bool redDotToggle = false;
    private Player player;
    void Start()
    {
        player = GetComponentInParent<Player>();
        FirstRoundDisplay.text = "Round 1";
        RoundText.text = "";
        PointsText.text = "500";
        FirstRoundDisplay.textWrappingMode = TextWrappingModes.PreserveWhitespace;
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

    public void UpdateRoundUI(int Round)
    {
        displayActive = true;
        RoundText.text = Round.ToString();
    }
    public void UpdatePointsUI()
    {
        PointsText.text = player.Points.ToString();
    }

    public void UpdateAmmoUI(string AmmoCount)
    {
        AmmoLeftText.text = AmmoCount;
    }


    public void UpdateAmmoReserveUI(string AmmoReserve)
    {
        AmmoReserverText.text = AmmoReserve;
    }


    public void ToggleCrosshair()
    {
        crosshairToggle = !crosshairToggle;
        Crosshair.SetActive(crosshairToggle);
    }

    public void ToggleRedDot()
    {
        redDotToggle = !redDotToggle;
        RedDot.SetActive(redDotToggle);
    }

}
