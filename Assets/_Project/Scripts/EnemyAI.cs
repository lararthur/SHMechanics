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
    private float currentSpeed = 0f; // Tracks the speed to be used in FixedUpdate

    // The State Machine
    public enum AIState { Idle, Chasing, Wandering }
    public AIState currentState = AIState.Idle;

    private EnemyHealth enemyHealth;
    private Rigidbody rb; // NEW: Reference to the Rigidbody

    void Start()
    {
        // Grab the health script so we know if this enemy is dead
        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody>(); // NEW: Grab the Rigidbody component

        // If you forgot to assign the player in the inspector, it will try to find it
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // If dead, stop everything
        if (enemyHealth != null && enemyHealth.isDead)
        {
            currentSpeed = 0f;
            return;
        }

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
        else
        {
            currentSpeed = 0f;
        }
    }

    void FixedUpdate()
    {
        // If the enemy is dead, don't execute physics movement
        if (enemyHealth != null && enemyHealth.isDead) return;

        // Walk forward applying movement safely inside the physics loop
        if (currentSpeed > 0.01f)
        {
            // Calculate the next position instead of translating directly
            Vector3 targetMovement = transform.forward * currentSpeed * Time.fixedDeltaTime;

            // MovePosition checks for wall collisions BEFORE moving, stopping the enemy instantly
            rb.MovePosition(rb.position + targetMovement);
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

        currentSpeed = chaseSpeed;
    }

    private void Wander()
    {
        // Just keep walking straight forward at a slower pace
        currentSpeed = wanderSpeed;
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