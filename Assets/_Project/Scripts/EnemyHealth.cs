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

        // Call the utility function! We pass 'transform', the 90-degree X offset, and the duration.
        yield return StartCoroutine(TransformUtils.RotationAnimation(transform, new Vector3(90f, 0f, 0f), deathAnimationDuration));
    }
}