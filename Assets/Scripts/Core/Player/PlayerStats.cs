using UnityEngine;
using Core.Managers;
using Core.Events;
using Core.Utils;
using Core.Characters.Health;

namespace Core.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PatchyMovement))]
    public class PlayerStats : MonoBehaviour
    {
        [Header("Experience Settings")]
        [SerializeField] private int currentExperience;

        [Header("Experience Consumption Settings")]
        [SerializeField] private int sprintExperienceCostPerSecond = 10; // Costo de experiencia por segundo de sprint
        [SerializeField] private int specialPowerExperienceCost = 300; // Costo de experiencia para poder especial

        public event System.Action<int> OnExperienceChanged;
        public int CurrentExperience => currentExperience;
        public int SprintExperienceCostPerSecond => sprintExperienceCostPerSecond;
        public int SpecialPowerExperienceCost => specialPowerExperienceCost;

        private void Awake()
        {
            InitializeStartingValues();
        }

        private void InitializeStartingValues()
        {
            currentExperience = 400;
            OnExperienceChanged?.Invoke(currentExperience);
            EventBus.Publish(new ExperienceChangedEventData(currentExperience));
            GameLogger.Log($"Initial experience set to: {currentExperience}");
        }

        public void AddExperience(int amount)
        {
            currentExperience += amount;
            OnExperienceChanged?.Invoke(currentExperience);
            EventBus.Publish(new ExperienceChangedEventData(currentExperience));
            Debug.Log($"[PlayerStats] Experience Added: {amount}, New Experience: {currentExperience}"); // Log the new total here
        }

        public void ConsumeExperience(int amount)
        {
            if (amount > 0)
            {
                currentExperience = Mathf.Max(0, currentExperience - amount);
                EventBus.Publish(new ExperienceChangedEventData(currentExperience));
                OnExperienceChanged?.Invoke(currentExperience);
                Debug.Log($"[PlayerStats] Experience Consumed: {amount}, New Experience: {currentExperience}");
            }
        }

        // Nuevo método para verificar si hay suficiente experiencia
        public bool HasEnoughExperience(int amount)
        {
            return currentExperience >= amount;
        }

        public bool CanUseSpecialPower()
        {
            return HasEnoughExperience(specialPowerExperienceCost);
        }

        public bool CanSprint()
        {
            return HasEnoughExperience(sprintExperienceCostPerSecond);
        }
    }
}
