// HealthRegeneration.cs
using UnityEngine;
using Core.Characters.Health;

[RequireComponent(typeof(Health))]
public class HealthRegeneration : MonoBehaviour
{
    [SerializeField] private int regenerationAmount = 1;
    [SerializeField] private float regenerationInterval = 1f;
    [SerializeField] private float regenerationDelay = 3f;

    private float regenerationTimer;
    private float intervalTimer;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        regenerationTimer = 0f;
        intervalTimer = 0f;
    }

    private void OnEnable()
    {
        health.OnHealthValueChanged += StartRegeneration;
    }

    private void OnDisable()
    {
        health.OnHealthValueChanged -= StartRegeneration;
    }

    private void Update()
    {
        if (regenerationTimer > 0)
        {
            regenerationTimer -= Time.deltaTime;
            if (regenerationTimer <= 0)
            {
                intervalTimer = regenerationInterval;
                RegenerateHealth();
            }
        }

        if (intervalTimer > 0)
        {
            intervalTimer -= Time.deltaTime;
            if (intervalTimer <= 0)
            {
                RegenerateHealth();
                intervalTimer = regenerationInterval;
            }
        }
    }

    private void RegenerateHealth()
    {
        if (health.CurrentHealth >= health.MaxHealth) return;

        health.Heal(regenerationAmount);

        if (health.CurrentHealth < health.MaxHealth)
        {
            StartRegeneration(health.CurrentHealth, health.MaxHealth);
        }
    }

    private void StartRegeneration(int current, int max)
    {
        regenerationTimer = regenerationDelay;
    }
}
