using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private PlayerStateMachine playerStateMachine;
    private UIManager PlayerUI;
    private Camera cam;
    private PlayerPoints playerPoints;
    private PlayerMotor playerMotor;
    private PlayerWeapon playerWeapon;

    public float meleeDamage;
    public float meleeDistance;
    public float meleeCooldown;
    public float meleeDuration;

    private InputManager inputManager;
    private float timeSinceLastMelee;
    private Animator animator;

    void Start()
    {
        playerStateMachine = Player.GetComponent<PlayerStateMachine>();
        PlayerUI = Player.GetComponentInChildren<UIManager>();
        cam = Player.GetComponentInChildren<Camera>();
        playerPoints = Player.GetComponent<PlayerPoints>();
        playerMotor = Player.GetComponentInChildren<PlayerMotor>();
        playerWeapon = Player.GetComponentInChildren<PlayerWeapon>();
        inputManager = GetComponentInParent<InputManager>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!playerStateMachine.canMelee)
        {
            timeSinceLastMelee += Time.deltaTime;
        }
        if (timeSinceLastMelee >= meleeCooldown)
        {
            playerStateMachine.canMelee = true;
            timeSinceLastMelee = 0;
        }

        if (timeSinceLastMelee >= meleeDuration)
        {
            playerStateMachine.isMeleeing = false;
            animator.SetBool("isMelee", false);
            //playerWeapon.GetComponentInChildren<MeshRenderer>().enabled = true;
        }
    }

    public void Melee()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if(playerStateMachine.canMelee)
        {
            playerWeapon = Player.GetComponentInChildren<PlayerWeapon>();
            playerMotor.CancelSprint();
            //playerWeapon.GetComponentInChildren<MeshRenderer>().enabled = false;
            playerStateMachine.isMeleeing = true;
            playerStateMachine.canMelee = false;
            animator.SetBool("isMelee", true);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, meleeDistance))
            {
                ZombieHealth target = hitInfo.transform.GetComponent<ZombieHealth>();
                if (target != null)
                {
                    if (target.health - meleeDamage <= 0)
                    {
                        playerPoints.Points += 130;
                    }

                    target.TakeDamage(meleeDamage);
                    //plus 10 for hit
                    playerPoints.Points += 10;
                    PlayerUI.UpdatePointsUI();
                }
            }
        }
    }
}
