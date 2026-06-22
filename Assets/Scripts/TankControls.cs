using UnityEngine;
using System.Collections; // Required for Coroutines!

public class TankControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f; // Faster speed for sprinting
    public float turnSpeed = 120f;

    // How long the 180 turn takes in seconds. 0.3 seconds is usually a sweet spot.
    public float quickTurnDuration = 0.3f;
    // A switch to lock the player's movement and aiming while they are turning
    private bool isQuickTurning = false;

    private float currentMoveSpeed; // Tracks whether we are currently walking or running
    private float moveInput;
    private float turnInput;

    [Header("Combat Settings")]
    public Transform gunBarrel; // Where the bullet comes from
    public int gunDamage = 1;
    public float weaponRange = 50f;
    private bool isAiming;

    void Start()
    {
        
    }

    void Update()
    {
        // 1. If we are currently spinning, force all inputs to zero and ignore the rest of the Update loop
        if (isQuickTurning)
        {
            moveInput = 0f;
            turnInput = 0f;
            return;
        }

        // Check if the player is holding the Aim key
        isAiming = Input.GetKey(KeyCode.F);

        if (isAiming)
        {
            // Lock movement entirely while aiming, but allow rotating to face enemies
            moveInput = 0f;
            turnInput = Input.GetAxisRaw("Horizontal");

            // Fire weapon when Spacebar is pressed (only works while aiming)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }
        else
        {
            // Normal exploration movement
            moveInput = Input.GetAxisRaw("Vertical");
            turnInput = Input.GetAxisRaw("Horizontal");

            // --- TRIGGER THE SMOOTH QUICK-TURN ---
            if (moveInput < -0.01f && Input.GetKeyDown(KeyCode.LeftShift))
            {
                // Start the Coroutine to handle the rotation over time
                StartCoroutine(PerformQuickTurn());
                return;
            }

            // If holding Left Shift AND moving forward/backward, use runSpeed. Otherwise, walkSpeed.
            if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(moveInput) > 0.01f)
            {
                currentMoveSpeed = runSpeed;
            }
            else
            {
                currentMoveSpeed = walkSpeed;
            }
        }
    }

    void FixedUpdate()
    {
        // Move via transform. Translate handles kinematic objects perfectly
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            Vector3 movement = transform.forward * moveInput * currentMoveSpeed * Time.fixedDeltaTime;
            transform.Translate(movement, Space.World);
        }

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
            transform.Rotate(0f, turn, 0f);
        }
    }

    void Shoot()
    {
        Debug.Log("Bang!");

        // Shoot an invisible raycast straight forward from the gun barrel
        RaycastHit hit;
        if (Physics.Raycast(gunBarrel.position, gunBarrel.forward, out hit, weaponRange))
        {
            // Check if the object we hit has the EnemyHealth script
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // Deal damage!
                enemy.TakeDamage(gunDamage);
            }
        }

        // This draws a temporary red line in the Scene view (not the Game view) so you can see your shots!
        Debug.DrawRay(gunBarrel.position, gunBarrel.forward * weaponRange, Color.red, 1f);
    }

    // --- COROUTINE FOR ANIMATED ROTATION ---
    private IEnumerator PerformQuickTurn()
    {
        // Lock the player's inputs at the very start of the spin
        isQuickTurning = true;

        Quaternion startRotation = transform.rotation;
        // Calculate exactly where they need to end up (current rotation + 180 degrees on Y)
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);

        float timeElapsed = 0f;

        // Loop this block of code until the duration is reached
        while (timeElapsed < quickTurnDuration)
        {
            // Slerp (Spherical Linear Interpolation) smoothly blends between two rotations based on time
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / quickTurnDuration);
            timeElapsed += Time.deltaTime;

            // "yield return null" tells Unity to pause this function here, render the frame, and come back next frame
            yield return null;
        }

        // Hard snap to the exact 180-degree rotation at the very end to prevent microscopic floating-point errors
        transform.rotation = targetRotation;

        // Unlock the player's inputs so they can move again
        isQuickTurning = false;
    }
}