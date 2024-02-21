using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    private Player player;
    private PlayerStateMachine playerStateMachine;
    private UIManager PlayerUI;
    private Camera cam;
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
        playerStateMachine = playerObj.GetComponent<PlayerStateMachine>();
        player = playerObj.GetComponent<Player>();
        PlayerUI = playerObj.GetComponentInChildren<UIManager>();
        cam = playerObj.GetComponentInChildren<Camera>();
        playerMotor = playerObj.GetComponentInChildren<PlayerMotor>();
        playerWeapon = playerObj.GetComponentInChildren<PlayerWeapon>();
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
            playerWeapon = playerObj.GetComponentInChildren<PlayerWeapon>();
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
                        player.Points += 130;
                    }

                    target.TakeDamage(meleeDamage);
                    //plus 10 for hit
                    player.Points += 10;
                    PlayerUI.UpdatePointsUI();
                }
            }
        }
    }
}
