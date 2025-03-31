using UnityEngine;
using System;
using Core.Utils;
using Core.Events;
using Core.Player;
using Core.Managers;

namespace Core.Characters.Health
{
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        private bool isDead = false;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;

        public event Action<int, int> OnHealthValueChanged;
        public event Action OnDeath;

        private void Awake()
        {
            ValidateHealth();
            currentHealth = maxHealth;
        }

        private void Start()
        {
            OnHealthValueChanged?.Invoke(currentHealth, maxHealth);
        }

        private void ValidateHealth()
        {
            if (maxHealth <= 0)
            {
                Debug.LogError($"Invalid max health value on {gameObject.name}");
                maxHealth = 100;
            }
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (isDead)
            {
                Debug.Log($"{gameObject.name} is already dead, can't take damage");
                return;
            }

            Debug.Log($"{gameObject.name} taking {damage} damage. Current health: {currentHealth}");
            currentHealth = Mathf.Max(0, currentHealth - damage);
            Debug.Log($"{gameObject.name} health after damage: {currentHealth}");
            OnHealthValueChanged?.Invoke(currentHealth, maxHealth);
            EventBus.Publish(new HealthChangedEventData(currentHealth, maxHealth));

            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                currentHealth = 0;
                GameLogger.Log($"{gameObject.name} has died!");
                Die();
            }
        }

        private void Die()
        {
            if (OnDeath != null)
            {
                GameLogger.Log($"Triggering death event for {gameObject.name}");
                OnDeath.Invoke();
            }
            else
            {
                GameLogger.LogWarning($"No death listeners for {gameObject.name}");
            }
            GameManager.Instance.GameOver();
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthValueChanged?.Invoke(currentHealth, maxHealth);
            EventBus.Publish(new HealthChangedEventData(currentHealth, maxHealth));
        }

        public void SetMaxHealth(int value)
        {
            maxHealth = Mathf.Max(1, value);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthValueChanged?.Invoke(currentHealth, maxHealth);
            EventBus.Publish(new HealthChangedEventData(currentHealth, maxHealth));
        }
    }
}
