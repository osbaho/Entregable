using Core.Characters.Health;
using UnityEngine;
using System.Collections;

public class SpecialPowerProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 9999; // Set a high damage value to ensure it kills the enemy
    [SerializeField] private float projectileSpeed = 15f; // Speed of the projectile
    [SerializeField] private float lifeTime = 3f; // Time before the projectile is destroyed
    [SerializeField] private float aimingDelay = 1f; // Delay for aiming

    private Rigidbody2D rb;
    private static bool specialPowerUsed = false; // Static variable to track if the special power has been used
    private static float nextSpecialPowerTime = 0f; // Static variable to track the next available time for special power
    private Vector2 targetDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Add a CollisionDetector component to the projectile
        CollisionDetector collisionDetector = gameObject.AddComponent<CollisionDetector>();
        // Set the collision layers for the CollisionDetector to only "Enemy" layer
        collisionDetector.CollisionLayers = LayerMask.GetMask("Enemy"); // Set the collision layers to only detect "Enemy" layer
        // Add a listener to the onCollision event
        collisionDetector.OnCollision += HandleCollision; // Subscribe to the custom event

        // Destroy the projectile after the specified lifetime
        Destroy(gameObject, lifeTime);

        // Start the aiming coroutine
        StartCoroutine(AimAndShoot());
    }

    private IEnumerator AimAndShoot()
    {
        // Wait for the aiming delay
        yield return new WaitForSeconds(aimingDelay);

        // Calculate the direction towards the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetDirection = (mousePosition - transform.position).normalized;

        // Set the initial velocity of the projectile towards the mouse
        rb.linearVelocity = targetDirection * projectileSpeed;
    }

    private void HandleCollision(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit by Special Power!");
            // Apply damage to the enemy
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Use the predefined damage value
            }
        }
    }

    public static bool CanUseSpecialPower()
    {
        // Check if the cooldown has finished
        if (specialPowerUsed && Time.time >= nextSpecialPowerTime)
        {
            ResetSpecialPower(); // Reset the special power if the cooldown is over
        }

        return !specialPowerUsed;
    }

    public static void UseSpecialPower()
    {
        specialPowerUsed = true;
        nextSpecialPowerTime = Time.time + 30f; // Set the next available time
    }

    public static void ResetSpecialPower()
    {
        specialPowerUsed = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticVariables()
    {
        specialPowerUsed = false;
        nextSpecialPowerTime = 0f;
    }
}
