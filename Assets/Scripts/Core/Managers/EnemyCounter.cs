using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] private Text counterText;
    private int enemiesAlive = 20; // Start with 20 enemies

    private void Start()
    {
        UpdateCounter();
    }

    public void EnemyKilled()
    {
        enemiesAlive--;
        UpdateCounter();

        if (enemiesAlive <= 0)
        {
            LoadFinishedScene();
        }
    }

    private void UpdateCounter()
    {
        counterText.text = enemiesAlive.ToString();
    }

    private void LoadFinishedScene()
    {
        Debug.Log("All enemies eliminated! Loading Finished scene.");
        SceneManager.LoadScene("Finished");
    }
}
