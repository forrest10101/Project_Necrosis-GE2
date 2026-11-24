
using UnityEngine;
using UnityEngine.AI;


// Zombie AI, Written by Andrew Forrest
// Zombie animations, written by Evan Montgomery
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

    private Animator animator;                                                                              // Zombie animator

    [SerializeField, Range(0f, 1f)] private float attackHitTime = 0.45f;                                    // Attack hit time float

    private static readonly int AttackState = Animator.StringToHash("Base Layer.Attack");                   // Attack state
    private const float AttackDuration = 1f;                                                                // Attack duration
    private bool isAttacking;                                                                               // Check if is attacking

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
        animator = GetComponentInChildren<Animator>();
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

        if (isAttacking)
        {
            agent.isStopped = true;
        }
        else if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            AttackPlayer();
        }

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            bool isMoving = !agent.isStopped && agent.velocity.sqrMagnitude > 0.01f;
            animator.SetBool("IsMoving", isMoving);
        }

        cooldown -= Time.deltaTime;
    }

    void AttackPlayer()
    {
        if (cooldown > 0f)
            return;

        cooldown = attackCooldown;

        if (animator == null || animator.runtimeAnimatorController == null ||
            !animator.HasState(0, AttackState))
        {
            playerHealth.PlayerDamage(Damage);
            return;
        }

        isAttacking = true;
        animator.Play(AttackState, 0, 0f);
        StartCoroutine(AttackSwing());
    }

    private System.Collections.IEnumerator AttackSwing()                                                // Attack swing animation
    {
        yield return new WaitForSeconds(attackHitTime);                                                 // Play animation

        if (isActiveAndEnabled && player != null && playerHealth != null &&
            Vector3.Distance(transform.position, player.position) <= attackRange)                       // If player is in range
        {
            playerHealth.PlayerDamage(Damage);                                                          // Player takes damage
        }

        yield return new WaitForSeconds(AttackDuration - attackHitTime);                                // Return to original animation
        isAttacking = false;                                                                            // Set attack state to false
    }
}