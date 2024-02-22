using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : NetworkBehaviour
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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Destroy(gameObject);
        }
    }

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
        if (!IsOwner)
        {
            return;
        }
        if (displayActive)
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
        if (!IsOwner)
        {
            return;
        }
        displayActive = true;
        RoundText.text = Round.ToString();
    }
    public void UpdatePointsUI()
    {
        if (!IsOwner)
        {
            return;
        }
        PointsText.text = player.Points.ToString();
    }

    public void UpdateAmmoUI(string AmmoCount)
    {
        if (!IsOwner)
        {
            return;
        }
        AmmoLeftText.text = AmmoCount;
    }


    public void UpdateAmmoReserveUI(string AmmoReserve)
    {
        if (!IsOwner)
        {
            return;
        }
        AmmoReserverText.text = AmmoReserve;
    }


    public void ToggleCrosshair()
    {
        if (!IsOwner)
        {
            return;
        }
        crosshairToggle = !crosshairToggle;
        Crosshair.SetActive(crosshairToggle);
    }

    public void ToggleRedDot()
    {
        if (!IsOwner)
        {
            return;
        }
        redDotToggle = !redDotToggle;
        RedDot.SetActive(redDotToggle);
    }

}
