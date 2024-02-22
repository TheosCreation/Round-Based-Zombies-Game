using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [Header("Cameras")]
    public Camera cam;
    [SerializeField] private AudioListener audioListener;
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;
    [Header("Text Reference")]
    [SerializeField] private TextMeshProUGUI promptText;

    private float health;
    [Header("Player Health")]
    public float maxHealth = 100;
    public float healthRecoverAmmount = 10;
    public float recoverTimer = 10;
    private float timeSinceLastHurt;
    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;

    private float durationTimer;

    [Header("Player Points")]
    public int Points = 500;

    
    [Header("Player Interactions")]
    public float interactionDistance = 3.0f;
    [SerializeField] private LayerMask mask;
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        inputManager = GetComponent<InputManager>();
        cam = GetComponentInChildren<Camera>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            audioListener.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        //player health
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health < maxHealth)
        {
            timeSinceLastHurt += Time.deltaTime;
        }
        if (timeSinceLastHurt >= recoverTimer)
        {
            RestoreHealth(healthRecoverAmmount);
            Debug.Log("Health Rocovered " + health);
            timeSinceLastHurt = 0;
        }
        if (health <= 0)
        {
            //change this asap
            Application.Quit();
        }
        if (overlay.color.a > 0)
        {
            if (health < 30)
            {
                return;
            }
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                //fade image
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
        // player Interactions
        UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, interactionDistance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable.tag == "WallBuy")
                {
                    WallBuy wallbuy = hitInfo.collider.GetComponent<WallBuy>();
                    PlayerWeapon playerWeapon = GetComponentInChildren<WeaponSwitching>().playerWeapon;
                    if (playerWeapon != null)
                    {
                        if (playerWeapon.tag != wallbuy.weaponPrefab.tag)
                        {
                            interactable.promptMessage = "Weapon Cost " + wallbuy.weaponCost.ToString();
                        }
                        else
                        {
                            interactable.promptMessage = "Refill Ammo " + wallbuy.replenshCost.ToString();
                        }
                    }
                }
                interactable.nearestPlayer = this;
                UpdateText(interactable.promptMessage);
                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
    public void UpdateHealthUI()
    {
        //Debug.Log(health);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        durationTimer = 0;
        timeSinceLastHurt = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
    }
}
