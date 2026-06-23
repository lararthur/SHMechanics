using UnityEngine;
using System.Collections; // Required for Coroutines!

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;
    public float deathAnimationDuration = 0.3f;
    public bool isDead = false;

    [Header("Visual Effects")]
    public GameObject bloodPrefab; // We will drop our particle system here

    // added 'Vector3 hitPoint' so the enemy knows exactly where it was shot!
    public void TakeDamage(int damageAmount, Vector3 hitPoint)
    {
        // Spawn the blood exactly at the bullet's impact point
        if (bloodPrefab != null)
        {
            Instantiate(bloodPrefab, hitPoint, Quaternion.identity);
        }

        // If they are already dead, ignore the damage (prevents multiple hits from overkill)
        if (isDead) return;

        health -= damageAmount;
        Debug.Log("Enemy hit! Health remaining: " + health);

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        Debug.Log("Enemy defeated!");
        isDead = true;

        Quaternion startRotation = transform.rotation;
        // Calculate exactly where they need to end up (current rotation + 90 degrees on X)
        Quaternion targetRotation = startRotation * Quaternion.Euler(90f, 0f, 0f);

        float timeElapsed = 0f;

        // Loop this block of code until the duration is reached
        while (timeElapsed < deathAnimationDuration)
        {
            // Slerp (Spherical Linear Interpolation) smoothly blends between two rotations based on time
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / deathAnimationDuration);
            timeElapsed += Time.deltaTime;

            // "yield return null" tells Unity to pause this function here, render the frame, and come back next frame
            yield return null;
        }

        transform.rotation = targetRotation;

        //StartCoroutine(LayDownAndDie());
        //Destroy(gameObject); // Removes the enemy from the scene
    }
}