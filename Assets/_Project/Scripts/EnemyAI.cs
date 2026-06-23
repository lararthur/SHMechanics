using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Targeting")]
    public Transform player;

    [Header("Distances")]
    public float aggroRadius = 5f;  // How close to wake up
    public float dropRadius = 8f;   // How far before it gives up

    [Header("Speeds")]
    public float chaseSpeed = 3f;
    public float wanderSpeed = 1.5f;
    public float turnSpeed = 5f;    // So it doesn't instantly snap to face you

    // The State Machine
    public enum AIState { Idle, Chasing, Wandering }
    public AIState currentState = AIState.Idle;

    private EnemyHealth enemyHealth;

    void Start()
    {
        // Grab the health script so we know if this enemy is dead
        enemyHealth = GetComponent<EnemyHealth>();

        // If you forgot to assign the player in the inspector, it will try to find it
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // If the enemy is dead, completely stop the AI from running
        if (enemyHealth != null && enemyHealth.isDead) return;

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // --- STATE TRANSITIONS ---
        // Should we wake up?
        if (currentState == AIState.Idle && distanceToPlayer <= aggroRadius)
        {
            currentState = AIState.Chasing;
        }
        // Should we give up?
        else if (currentState == AIState.Chasing && distanceToPlayer > dropRadius)
        {
            currentState = AIState.Wandering;
        }

        // --- STATE BEHAVIORS ---
        if (currentState == AIState.Chasing)
        {
            ChasePlayer();
        }
        else if (currentState == AIState.Wandering)
        {
            Wander();
        }
    }

    private void ChasePlayer()
    {
        // Find the direction to the player, but lock the Y axis so the enemy doesn't lean backward/forward
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        // Smoothly rotate to face the player
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        // Walk forward
        transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime, Space.Self);
    }

    private void Wander()
    {
        // Just keep walking straight forward at a slower pace
        transform.Translate(Vector3.forward * wanderSpeed * Time.deltaTime, Space.Self);
    }

    // --- EDITOR VISUALS ---
    // This draws circles in your Scene view so you can visually tune the radius distances!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }
}