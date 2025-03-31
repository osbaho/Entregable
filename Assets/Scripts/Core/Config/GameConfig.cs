using UnityEngine;
using Core.Config;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
    public class GameConfig : ConfigBase
    {
        [Header("Game Settings")]
        [SerializeField] private float gameOverDelay = 2f;
        [SerializeField] private string initialScene = "MainMenu";
        
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showGizmos = true;

        public float GameOverDelay => gameOverDelay;
        public string InitialScene => initialScene;
        public bool EnableDebugLogs => enableDebugLogs;
        public bool ShowGizmos => showGizmos;

        protected override void ValidateFields()
        {
            ValidateGreaterThanZero(ref gameOverDelay, nameof(gameOverDelay));
            if (string.IsNullOrEmpty(initialScene))
            {
                initialScene = "MainMenu";
            }
        }
    }
}
