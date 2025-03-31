using UnityEngine;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour
{
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        if (quitButton == null)
        {
            Debug.LogError("Quit Button not assigned in the Inspector!");
            return;
        }
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
