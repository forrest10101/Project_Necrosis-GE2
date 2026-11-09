
using UnityEngine;
using UnityEngine.AI;


//ZombieAI, Written by Andrew Forrest
[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Combat")]
    public int Damage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private float cooldown;
    private Transform player;
    private PlayerHealth playerHealth;
    private NavMeshAgent agent;

    void Start()
    {
        if (CompareTag("NormZombie"))
        {
            Damage = 3;
            speed = 4f;
        }

        else if (CompareTag("UpgradeZombie"))
        {
            Damage = 5;
            speed = 5.8f;
        }

        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.stoppingDistance = attackRange;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null)
            return;

        agent.speed = speed;
        agent.stoppingDistance = attackRange;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            AttackPlayer();
        }

        cooldown -= Time.deltaTime;
    }

    void AttackPlayer()
    {
        if (cooldown <= 0f)
        {
            playerHealth.PlayerDamage(Damage);
            cooldown = attackCooldown;
        }
    }
}