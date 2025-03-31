using Core.Characters.Health;
using UnityEngine;
using Core.Player;
using Core.Enemy;
using Core.Managers; // Add this line for EnemyCounter

public class EnemyDeathHandler : MonoBehaviour
{
    private Health health;
    [SerializeField] private EnemyExperienceConfig experienceConfig;
    private EnemyCounter enemyCounter; // Add this line

    private void Awake()
    {
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError("Health component not found on this GameObject!");
        }
        if (experienceConfig == null)
        {
            Debug.LogError("EnemyExperienceConfig not found on this GameObject!");
        }
        // Find the EnemyCounter in the scene
        enemyCounter = FindAnyObjectByType<EnemyCounter>(); // Add this line
        if (enemyCounter == null) // Add this line
        { // Add this line
            Debug.LogError("EnemyCounter not found in the scene!"); // Add this line
        } // Add this line
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        // Find the PlayerStats component in the player
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>(); // Using FindAnyObjectByType
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found in the scene!");
            return;
        }
        if (experienceConfig == null)
        {
            Debug.LogError("EnemyExperienceConfig not found!");
            return;
        }
        // Add experience to the player
        int experienceToAdd = experienceConfig.ExperienceReward;
        playerStats.AddExperience(experienceToAdd);
        Debug.Log($"Added {experienceToAdd} experience to player.");

        // Notify the EnemyCounter that an enemy has been killed
        if (enemyCounter != null) // Add this line
        { // Add this line
            enemyCounter.EnemyKilled(); // Add this line
        } // Add this line

        Destroy(gameObject);
    }
}
