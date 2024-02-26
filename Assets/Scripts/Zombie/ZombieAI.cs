using System;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : NetworkBehaviour
{
    public float health;
    public float maxHealth;

    private StateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent;}
    
    [SerializeField]
    private string currentState;
    public GameObject target;
    public GameObject targetLook;
    public float distanceToPlayer;
    public float attackRange = 2.0f;
    public float attackCycleTimer = 1.5f;
    public float attackRate = 2.0f;
    public float attackDamage = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        
        stateMachine.Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health <= 0)
        {
            RoundSpawner.Instance.ZombieKilled(gameObject);
        }
        PlayerInAttackRange();
        currentState = stateMachine.activeState.ToString();
    }

    public bool PlayerInAttackRange()
    {
        distanceToPlayer = MathF.Abs(transform.position.x - target.transform.position.x) + MathF.Abs(transform.position.z - target.transform.position.z);
        
        if(distanceToPlayer <= attackRange)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void DestroySelf()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }    
}
