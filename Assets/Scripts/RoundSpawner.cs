using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RoundSpawner : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private GameObject[] zombiePrefabs;
    [SerializeField] private UIManager playerUI;
    [SerializeField] private PlayerPoints playerPoints;
    [SerializeField] private int baseZombies = 8;
    //starts at 1 zombie per 2 seconds and at round 60 max cap of 10 zombies per second so zombiesPerSecond = 10 at round 60 and zombiesPerSecond = 5 at round 30
    [SerializeField] private float zombiesPerSecond = 0.5f;
    [SerializeField] private float initialRoundStartTime;
    [SerializeField] private float timeBetweenRounds;
    [SerializeField] private float healthIncreaseMultiplier = 1.1f;


    [Header("Audio")]
    public AudioSource audioInCamera;
    public AudioClip roundStartClip;
    public AudioClip roundEndClip;

    [Header("Events")]
    public static UnityEvent onZombieKilled = new UnityEvent();

    public int currentRound = 1;
    private float timeToStartRound;
    private float timeSinceLastSpawn;
    private int zombiesAlive;
    private int zombiesLeftToSpawn;
    private int maxZombiesThisRound;
    private int zombiesAllowedAlive;
    private int baseZombiesAllowedAlive = 24;
    private int zombiesCurrentHealth = 50;
    private bool isSpawning = false;



    void Awake()
    {
        onZombieKilled.AddListener(ZombieKilled);
    }
    void Start()
    {
        playerUI.GetComponent<PlayerUI>();
        zombiesLeftToSpawn = baseZombies;
        //play start round sound
        timeToStartRound = initialRoundStartTime;
        StartCoroutine(StartRound());
        timeToStartRound = timeBetweenRounds;
    }

    void Update()
    {
        if(!isSpawning)
        {
            return;
        }            
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= (1f / zombiesPerSecond) && zombiesLeftToSpawn > 0 && zombiesAlive < zombiesAllowedAlive)
        {
            SpawnZombie();
            zombiesAlive++;
            zombiesLeftToSpawn--;
            timeSinceLastSpawn = 0;
        }

        if(zombiesAlive == 0 && zombiesLeftToSpawn == 0)
        {
            EndRound();
        }
    }
    private void ZombieKilled()
    {
        playerUI.UpdatePointsUI(playerPoints.Points);
        zombiesAlive--;
    }

    private IEnumerator StartRound()
    {
        yield return new WaitForSeconds(timeToStartRound);
        audioInCamera.PlayOneShot(roundStartClip);
        //update round count text
        playerUI.UpdateRoundUI(currentRound);

        maxZombiesThisRound = zombiesLeftToSpawn;
        zombiesPerSecond = CalculateZombieRate(currentRound);
        if(currentRound == 2)
        {
            zombiesLeftToSpawn = 8;
        }
        if(currentRound == 3)
        {
            zombiesLeftToSpawn = 13;
        }
        if(currentRound == 4)
        {
            zombiesLeftToSpawn = 18;
        }
        if(currentRound == 5)
        {
            zombiesLeftToSpawn = 24;
        }
        if(currentRound == 6)
        {
            zombiesLeftToSpawn = 27;
        }
        if(currentRound == 7)
        {
            zombiesLeftToSpawn = 28;
        }
        if (currentRound == 9)
        {
            zombiesLeftToSpawn = 29;
        }
        if (currentRound == 10)
        {
            zombiesLeftToSpawn = 33;
        }
        if (currentRound == 11)
        {
            zombiesLeftToSpawn = 34;
        }
        if (currentRound == 12)
        {
            zombiesLeftToSpawn = 36;
        }
        if (currentRound <= 4)
        {
            zombiesAllowedAlive = Mathf.RoundToInt((float)((currentRound * 0.2f) * baseZombiesAllowedAlive));
        }else if(currentRound == 5)
        {
            zombiesAllowedAlive = baseZombiesAllowedAlive;
        }else if (currentRound >= 10)
        {
            zombiesAllowedAlive = Mathf.RoundToInt((float)(currentRound * (0.15f) * baseZombiesAllowedAlive));
        }
        if (currentRound > 12)
        {
            zombiesLeftToSpawn = ZombiesPerRoundAfterRound12();
        }
        if (currentRound <= 9)
        {
            zombiesCurrentHealth += 100;
        }
        else
        {
            zombiesCurrentHealth = Mathf.RoundToInt(zombiesCurrentHealth * healthIncreaseMultiplier);
        }
        StartCoroutine(StartSpawning());
    }

    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(2*initialRoundStartTime);
        isSpawning = true;
    }

    private void EndRound()
    {
        //play sound to let player know round has ended
        audioInCamera.PlayOneShot(roundEndClip);
        isSpawning = false;
        timeSinceLastSpawn = 0;
        currentRound++;
        StartCoroutine(StartRound());
    }

    private void SpawnZombie()
    {
        GameObject prefabToSpawn = zombiePrefabs[0];
        prefabToSpawn.GetComponent<ZombieHealth>().maxHealth = zombiesCurrentHealth;

        //REVIST THIS PLEASE NOT WORKING
        Vector3 spawnPosition = new Vector3(Mathf.Clamp(player.transform.position.x + Random.Range(-15,15), -50, 50), 1, Mathf.Clamp(player.transform.position.z + Random.Range(-15,15), -50, 50));
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    private int ZombiesPerRoundAfterRound12()
    {
        return Mathf.RoundToInt((float)(0.08685 * Mathf.Pow(currentRound, 2) + (0.1954 * currentRound) + 22.05));
    }

    private float CalculateZombieRate(float roundCount)
    {
        float[] coefficients = { -1.48970867e-05f, 1.98093085e-03f, -1.00859166e-01f, 2.09316232f };
        float zombieRate = 0f;

        for (int i = 0; i < coefficients.Length; i++)
        {
            zombieRate += coefficients[i] * Mathf.Pow(roundCount, coefficients.Length - 1 - i);
        }

        return 1/zombieRate;
    }

}