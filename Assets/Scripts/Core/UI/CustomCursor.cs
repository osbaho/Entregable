using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private GameObject cursorPrefab;
    private GameObject currentCursor;
    private Camera mainCamera;

    private void Awake()
    {
        Debug.Log("CustomCursor Awake called");
        // Hide the system cursor
        Cursor.visible = false;
        mainCamera = Camera.main;

        // Instantiate the cursor prefab
        currentCursor = Instantiate(cursorPrefab);
        if (currentCursor == null)
        {
            Debug.LogError("Failed to instantiate cursor prefab!");
        }
        else
        {
            Debug.Log("Cursor prefab instantiated successfully.");
        }
    }

    private void Update()
    {
        // Get the mouse position in world coordinates
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Update the position of the custom cursor
        if (currentCursor != null)
        {
            currentCursor.transform.position = mousePosition;
        }
        else
        {
            Debug.LogWarning("currentCursor is null in Update!");
        }
    }

    private void OnDestroy()
    {
        // Restore the system cursor when this object is destroyed
        Cursor.visible = true;

        if (currentCursor != null)
        {
            Destroy(currentCursor);
        }            
    }
}
