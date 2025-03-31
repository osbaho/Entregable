using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class MenuController : MonoBehaviour
{
    private const string MISSION1_SCENE = "Mision1"; 

    [SerializeField] private Button playButton; 

    private void OnEnable()
    {
        if (playButton == null)
        {
            Debug.LogError("Play Button not assigned in the Inspector!");
            return;
        }
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnDisable()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log($"Loading scene: {MISSION1_SCENE}");
        SceneManager.LoadScene(MISSION1_SCENE);
    }
}
