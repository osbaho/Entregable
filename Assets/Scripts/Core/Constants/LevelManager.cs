using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Configuración de Mapas")]
    [Tooltip("Prefabs de mapas disponibles")]
    [SerializeField] private List<GameObject> mapPrefabs = new List<GameObject>();
    [SerializeField] private Transform mapContainer;
    [SerializeField] private float xOffset = 20f; // Nuevo: offset configurable en X
    private HashSet<string> usedTriggers = new HashSet<string>();
    private List<int> recentlyUsedMaps = new List<int>();
    private const float LOW_PROBABILITY = 0.05f;

    void Awake()
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

    private int SelectMapWithWeightedProbability()
    {
        // Crear lista de pesos: 1.0 para mapas no usados, 0.05 para usados recientemente
        List<float> weights = new List<float>();
        float totalWeight = 0;

        for (int i = 0; i < mapPrefabs.Count; i++)
        {
            float weight = recentlyUsedMaps.Contains(i) ? LOW_PROBABILITY : 1.0f;
            weights.Add(weight);
            totalWeight += weight;
        }

        // Selección ponderada
        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            currentSum += weights[i];
            if (randomValue <= currentSum)
            {
                // Actualizar lista de mapas recientes
                recentlyUsedMaps.Add(i);
                if (recentlyUsedMaps.Count > 3) // Mantener solo los últimos 3 mapas
                {
                    recentlyUsedMaps.RemoveAt(0);
                }
                return i;
            }
        }

        return 0; // Fallback al primer mapa
    }

    public void LoadRandomMapFromTrigger(string triggerId, Vector3 spawnPosition)
    {
        if (usedTriggers.Contains(triggerId))
        {
            Debug.Log("Este trigger ya generó un mapa");
            return;
        }

        if (mapPrefabs.Count == 0)
        {
            Debug.LogError("No hay prefabs de mapas asignados");
            return;
        }

        int selectedIndex = SelectMapWithWeightedProbability();
        GameObject newMap = Instantiate(mapPrefabs[selectedIndex], mapContainer);
        usedTriggers.Add(triggerId);
        
        // Aplicar offset en X manteniendo Y y Z originales
        Vector3 offsetPosition = new Vector3(spawnPosition.x + xOffset, spawnPosition.y, spawnPosition.z);
        newMap.transform.position = offsetPosition;
    }

    public void ResetTrigger(string triggerId)
    {
        usedTriggers.Remove(triggerId);
    }
}