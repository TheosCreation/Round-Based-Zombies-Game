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

    [SerializeField] private float timeBetweenRounds = 4f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    [Header("Audio")]
    public AudioSource audioInCamera;
    public AudioClip changeRoundSound;

    [Header("Events")]
    public static UnityEvent onZombieKilled = new UnityEvent();

    public int currentRound = 1;
    private float timeSinceLastSpawn;
    private int zombiesAlive;
    private int zombiesLeftToSpawn;
    private int maxZombiesAllowedAlive;
    private int maxZombiesThisRound;
    private bool isSpawning = false;
    void Awake()
    {
        onZombieKilled.AddListener(ZombieKilled);
    }
    void Start()
    {
        playerUI.GetComponent<PlayerUI>();
        StartCoroutine(StartRound());
    }

    void Update()
    {
        if(!isSpawning)
        {
            return;
        }            
        timeSinceLastSpawn += Time.deltaTime;
        
        if(timeSinceLastSpawn >= (1f / zombiesPerSecond) && zombiesLeftToSpawn > 0 && zombiesAlive <= maxZombiesAllowedAlive)
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
        playerPoints.Points += 30;
        playerUI.UpdatePointsUI(playerPoints.Points);
        zombiesAlive--;
    }

    private IEnumerator StartRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        //spawn the round counter on screen and then display in top left corner of screen
        if(currentRound == 1)
        {
            playerUI.FirstRoundAlert();
        }
        else{
            playerUI.UpdateRoundUI(currentRound);
        }
        isSpawning = true;
        zombiesLeftToSpawn = ZombiesPerRound();
        maxZombiesThisRound = zombiesLeftToSpawn;
        ZombiesPerSecond();
        maxZombiesAllowedAlive = MaxZombiesAllowedAlive();
    }

    private void EndRound()
    {
        //play sound to let player know round has ended
        audioInCamera.PlayOneShot(changeRoundSound);
        isSpawning = false;
        timeSinceLastSpawn = 0;
        currentRound++;
        StartCoroutine(StartRound());
    }

    private void SpawnZombie()
    {
        GameObject prefabToSpawn = zombiePrefabs[0];
        //spawn position is a random location relevant to the player
        Vector3 spawnPosition = new Vector3(Mathf.Clamp(player.transform.position.x + Random.Range(-15,15), -50, 50), 1, Mathf.Clamp(player.transform.position.z + Random.Range(-15,15), -50, 50));
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    private int ZombiesPerRound()
    {
        return Mathf.RoundToInt(baseZombies * Mathf.Pow(currentRound, difficultyScalingFactor));
    }

    private int MaxZombiesAllowedAlive()
    {
        return 2 * currentRound + 2;
    }

    private void ZombiesPerSecond()
    {
        zombiesPerSecond = (1f/6f *  currentRound) + 1f/3f;
    }
}