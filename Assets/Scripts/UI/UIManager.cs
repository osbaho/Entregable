using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Core.Characters.Health;
using Core.Managers;
using Core.Utils;
using Core.Constants;
using PlayerStats = Core.Player.PlayerStats;
using Core.Characters;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        [Header("Health UI")]
        [SerializeField] private Image healthBarImage;

        [Header("Experience UI")]
        [SerializeField] private Image experienceBarImage;

        [Header("References")]
        [SerializeField] private PlayerStats playerReference;
        [SerializeField] private Health playerHealth;

        private int maxExperienceReached = 0;

        private void Awake()
        {
            ValidateAndInitialize();
            ServiceLocator.Instance.RegisterService<IUIManager>(this);
        }

        private void ValidateAndInitialize()
        {
            if (!ValidateReferences())
            {
                enabled = false;
                return;
            }
            ValidateUIElements();
            SubscribeToEvents();
            InitializeUI();
        }

        private bool ValidateReferences()
        {
            bool isValid = true;
            if (playerReference == null)
            {
                playerReference = FindAnyObjectByType<PlayerStats>();
                GameLogger.Log($"Attempting to find PlayerStats: {(playerReference != null ? "Found" : "Not Found")}",
                    playerReference == null ? GameLogger.LogLevel.Warning : GameLogger.LogLevel.Info);
                isValid &= playerReference != null;
            }
            else
            {
                GameLogger.Log($"PlayerStats already assigned: {playerReference.name}");
            }
            if (playerReference != null)
            {
                playerHealth = playerReference.GetComponent<Health>();
                GameLogger.Log($"Getting Health component: {(playerHealth != null ? "Success" : "Failed")}",
                    playerHealth == null ? GameLogger.LogLevel.Warning : GameLogger.LogLevel.Info);
                isValid &= playerHealth != null;
            }
            else
            {
                GameLogger.LogWarning("PlayerReference is null, cannot get Health component.");
            }
            return isValid;
        }

        private void ValidateUIElements()
        {
            ValidateUIElement(healthBarImage, "Health Bar Image");
            ValidateUIElement(experienceBarImage, "Experience Bar Image");
        }

        private void ValidateUIElement(Object element, string elementName)
        {
            if (element == null)
                GameLogger.Log($"{elementName} is not assigned in UIManager", GameLogger.LogLevel.Warning);
        }

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<HealthChangedEventData>(OnHealthChanged);
            EventBus.Subscribe<ExperienceChangedEventData>(OnExperienceChanged);
            EventBus.Subscribe<CharacterDeathEventData>(OnCharacterDeath);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver += LoadGameOverScene;
            }
            if (playerHealth != null)
            {
                playerHealth.OnHealthValueChanged += UpdateHealthUI;
            }
            playerReference.OnExperienceChanged += UpdateExperienceUI;
        }

        private void InitializeUI()
        {
            if (healthBarImage != null && playerHealth != null)
            {
                healthBarImage.fillAmount = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
            }
            if (experienceBarImage != null && playerReference != null)
            {
                maxExperienceReached = playerReference.CurrentExperience;
                UpdateExperienceBarUI(playerReference.CurrentExperience);
            }
        }

        public void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            if (healthBarImage != null)
            {
                Debug.Log($"Updating Health UI: currentHealth={currentHealth}, maxHealth={maxHealth}");
                healthBarImage.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
                Debug.Log($"healthBarImage.fillAmount: {healthBarImage.fillAmount}");
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(healthBarImage.rectTransform);
            }
        }

        public void UpdateExperienceUI(int experience)
        {
            UpdateExperienceBarUI(experience);
        }

        private void UpdateExperienceBarUI(int experience)
        {
            if (experienceBarImage != null)
            {
                maxExperienceReached = Mathf.Max(maxExperienceReached, experience);
                float fillAmount = (float)experience / maxExperienceReached;
                experienceBarImage.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }

        private void OnHealthChanged(HealthChangedEventData data)
        {
            Debug.Log("OnHealthChanged called!");
            UpdateHealthUI(data.CurrentHealth, data.MaxHealth);
        }

        private void OnExperienceChanged(ExperienceChangedEventData data)
        {
            UpdateExperienceUI(data.Experience);
        }

        private void OnCharacterDeath(CharacterDeathEventData data)
        {
            if (data.Character.CompareTag("Player"))
            {

            }
        }

        private void LoadGameOverScene()
        {
            Debug.Log("Loading GameOver scene.");
            SceneManager.LoadScene("GameOver");
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<HealthChangedEventData>(OnHealthChanged);
            EventBus.Unsubscribe<ExperienceChangedEventData>(OnExperienceChanged);
            EventBus.Unsubscribe<CharacterDeathEventData>(OnCharacterDeath);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver -= LoadGameOverScene;
            }
            if (playerHealth != null)
                playerHealth.OnHealthValueChanged -= UpdateHealthUI;
            if (playerReference != null)
                playerReference.OnExperienceChanged -= UpdateExperienceUI;
        }
    }
}
