using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Button

public class ExitButtonController : MonoBehaviour
{
    private const string MAIN_MENU_SCENE = "MainMenu"; // Define the MainMenu scene name as a constant

    [SerializeField] private Button exitButton; // Assign the button in the Inspector

    private void OnEnable()
    {
        if (exitButton == null)
        {
            Debug.LogError("Exit Button not assigned in the Inspector!");
            return;
        }
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnDisable()
    {
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }

    private void OnExitButtonClicked()
    {
        Debug.Log($"Loading scene: {MAIN_MENU_SCENE}");
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
}
