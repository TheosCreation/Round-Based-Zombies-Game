using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class ZombiesGameMultiplayer : NetworkBehaviour
{
    public static ZombiesGameMultiplayer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnZombie(GameObject zombiePrefab, float zombiesCurrentHealth, float zombiesMoveSpeed, Vector3[] spawnAroundPoints)
    {
        zombiePrefab.GetComponent<ZombieHealth>().maxHealth = zombiesCurrentHealth;
        zombiePrefab.GetComponent<NavMeshAgent>().speed = zombiesMoveSpeed;
        Vector3 spawnPositionSelected = spawnAroundPoints[Random.Range(0, spawnAroundPoints.Length)];

        Vector3 spawnPosition = new Vector3(spawnPositionSelected.x + Random.Range(-4, 4), 1, spawnPositionSelected.z + Random.Range(-4, 4));
        GameObject NewZombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        NetworkObject zombieNetworkObject = NewZombie.GetComponent<NetworkObject>();
        zombieNetworkObject.Spawn(true);
        //Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
