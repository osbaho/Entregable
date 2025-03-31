using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject PanelPause;
    public Button BtnResume, BtnMainMenu, BtnQuit;

    private bool isPaused = false;

    void Start()
    {
        BtnResume.onClick.AddListener(Resume);
        BtnMainMenu.onClick.AddListener(GoToMainMenu);
        BtnQuit.onClick.AddListener(QuitGame);

        PanelPause.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        PanelPause.SetActive(false);
        ResetTimeScale();
        isPaused = false;
    }

    public void Pause()
    {
        PanelPause.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void GoToMainMenu()
    {
        LoadMainMenu();
    }

    public void QuitGame()
    {
        LoadMainMenu();
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void LoadMainMenu()
    {
        ResetTimeScale();
        SceneManager.LoadScene("MainMenu");
    }
}
