using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FirstRoundDisplay;
    [SerializeField] private TextMeshProUGUI RoundText;
    [SerializeField] private TextMeshProUGUI PointsText;
    [SerializeField] private TextMeshProUGUI AmmoLeftText;
    [SerializeField] private GameObject Crosshair;
    [SerializeField] private GameObject RedDot;
    [SerializeField] private PlayerWeapon Weapon;
    private float displayTimer = 0f;
    private float timeToDisplay = 3.5f;
    private bool displayActive = false;
    private bool displayedRound1 = false;
    private bool crosshairToggle  = true;
    private bool redDotToggle = false;
    void Start()
    {
        FirstRoundDisplay.text = "";
        RoundText.text = "";
        PointsText.text = "0";
        FirstRoundDisplay.enableWordWrapping = true;
    }
    void Update()
    {
        UpdateAmmoUI();
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
    public void UpdatePointsUI(int Points)
    {
        PointsText.text = Points.ToString();
    }

    public void UpdateAmmoUI()
    {
        AmmoLeftText.text = Weapon.ammoLeft.ToString();
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
