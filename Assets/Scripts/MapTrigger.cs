using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    private string triggerId;
    [SerializeField] private Vector3 mapOffset = Vector3.right * 20f; // Ajusta esto en el Inspector

    private void Awake()
    {
        triggerId = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 spawnPosition = transform.position + mapOffset;
            LevelManager.Instance.LoadRandomMapFromTrigger(triggerId, spawnPosition);
        }
    }
}
