using UnityEngine;

public class ScreenPositionValidator : MonoBehaviour
{
    public bool IsMousePositionValid()
    {
        Vector3 mousePosition = Input.mousePosition;
        return mousePosition.x >= 0 && mousePosition.x <= Screen.width &&
               mousePosition.y >= 0 && mousePosition.y <= Screen.height;
    }
}