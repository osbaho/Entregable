using System;
using UnityEngine;

namespace Core.Utils
{
    public static class GameLogger
    {
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log($"[INFO] {message}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[WARNING] {message}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"[ERROR] {message}");
                    break;
                default:
                    Debug.Log($"[LOG] {message}");
                    break;
            }
        }

        public static void LogError(string message, Exception e = null)
        {
            if (e != null)
            {
                Debug.LogError($"[ERROR] {message}: {e.Message}\n{e.StackTrace}");
            }
            else
            {
                Debug.LogError($"[ERROR] {message}");
            }
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[WARNING] {message}");
        }
    }
}
