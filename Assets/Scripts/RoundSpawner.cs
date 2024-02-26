using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RoundSpawner : NetworkBehaviour
{

    [Header("Player Level Variables")]
    public GameObject[] players;
    public Vector3 SpawnPosition;
    public Quaternion SpawnRotation;
    [Header("Zombies Spawning")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Vector3[] spawnAroundPoints;
    [SerializeField] private int baseZombies = 8;
    //starts at 1 zombie per 2 seconds and at round 60 max cap of 10 zombies per second so zombiesPerSecond = 10 at round 60 and zombiesPerSecond = 5 at round 30
    [SerializeField] private float zombiesPerSecond = 0.5f;
    [SerializeField] private float initialRoundStartTime;
    [SerializeField] private float timeBetweenRounds;
    [SerializeField] private float healthIncreaseMultiplier = 1.1f;
    [SerializeField] private float zombiesMoveSpeedAdd = 0.2f;

    [Header("Audio")]
    public AudioSource audioInCamera;
    public AudioClip roundStartClip;
    public AudioClip roundEndClip;

    [Header("Rounds")]
    public int currentRound = 1;
    private float timeToStartRound;
    private float timeSinceLastSpawn;
    private int zombiesAlive;
    private int zombiesLeftToSpawn;
    private int maxZombiesThisRound;
    private int zombiesAllowedAlive;
    private int baseZombiesAllowedAlive = 24;
    private int zombiesCurrentHealth = 150;
    private float zombiesMoveSpeed = 2.0f;
    private bool isSpawning = false;
    public static RoundSpawner Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        
        zombiesLeftToSpawn = baseZombies;
        //play start round sound
        timeToStartRound = initialRoundStartTime;
        StartCoroutine(StartRound());
        timeToStartRound = timeBetweenRounds;
    }

    void Update()
    {
        if(!IsServer)
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

        if(isSpawning && zombiesAlive == 0 && zombiesLeftToSpawn == 0)
        {
            EndRound();
        }
    }
    public void ZombieKilled(GameObject zombie)
    {
        foreach (GameObject player in players)
        {
            player.GetComponentInChildren<UIManager>().UpdatePointsUI();
        }
        //zombie.GetComponent<NetworkObject>().Despawn();
        DestroyZombieObject(zombie);
        zombiesAlive--;
    }

    private IEnumerator StartRound()
    {
        yield return new WaitForSeconds(timeToStartRound);

        audioInCamera.PlayOneShot(roundStartClip);
        //update round count text
        foreach (GameObject player in players)
        {
            player.GetComponentInChildren<UIManager>().UpdateRoundUI(currentRound);
        }

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
        if (currentRound == 8)
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
        }
        else
        {
            zombiesAllowedAlive = baseZombiesAllowedAlive;
        }
        if (currentRound > 12)
        {
            zombiesLeftToSpawn = ZombiesPerRoundAfterRound12();
        }
        if(currentRound > 1)
        {
            if (currentRound <= 9)
            {
                zombiesCurrentHealth += 100;
                zombiesMoveSpeed += zombiesMoveSpeedAdd;
            }
            else
            {
                zombiesCurrentHealth = Mathf.RoundToInt(zombiesCurrentHealth * healthIncreaseMultiplier);
            }
        }
        
        maxZombiesThisRound = zombiesAllowedAlive;
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
        ZombiesGameMultiplayer.Instance.SpawnZombie(zombiePrefab, zombiesCurrentHealth, zombiesMoveSpeed, spawnAroundPoints);
    }

    private void DestroyZombieObject(GameObject zombieObject)
    {
        ZombiesGameMultiplayer.Instance.DestroyZombieObject(zombieObject);
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