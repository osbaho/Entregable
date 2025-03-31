using Core.Config;
using UnityEngine; 
using Core.Utils;

namespace Core.Managers
{
    public class GameConfigManager : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;

        private void Awake()
        {
            if (gameConfig == null)
            {
                GameLogger.Log("GameConfig not assigned!", GameLogger.LogLevel.Error);
                enabled = false;
                return;
            }

            ServiceLocator.Instance.RegisterService(gameConfig); // Changed this line
        }

        private void OnDestroy()
        {
            //ServiceLocator.Clear(); // Commented out this line
        }
    }
}