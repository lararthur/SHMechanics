using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log("Enemy hit! Health remaining: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy defeated!");
        Destroy(gameObject); // Removes the enemy from the scene
    }
}