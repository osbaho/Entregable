using System;
using UnityEngine;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action OnGameOver;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void GameOver()
        {
            Debug.Log("[GameManager] GameOver called.");
            OnGameOver?.Invoke();
        }
    }
}
