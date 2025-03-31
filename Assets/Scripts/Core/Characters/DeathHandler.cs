using UnityEngine;
using Core.Player;
using Core.Utils;
using Core.Managers;

namespace Core.Characters
{
    [RequireComponent(typeof(PlayerStats))]
    public class DeathHandler : EventSubscriber
    {
        private PlayerStats playerStats;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
        }

        protected override void SubscribeToEvents()
        {
            playerStats.OnExperienceChanged += HandleExperienceChanged;
        }

        protected override void UnsubscribeFromEvents()
        {
            if (playerStats != null)
            {
                playerStats.OnExperienceChanged -= HandleExperienceChanged;
            }
        }

        private void HandleExperienceChanged(int experience)
        {
            Debug.Log($"[DeathHandler] HandleExperienceChanged called. Current Experience: {experience}");
            if (experience <= 0)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}
