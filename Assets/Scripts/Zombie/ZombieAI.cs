using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
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
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        
        stateMachine.Initialise();
    }

    // Update is called once per frame
    void Update()
    {
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
}
