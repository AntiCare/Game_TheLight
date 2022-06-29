using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTools
{
    public static Vector3 GetMousePositionWhereCameraZIs70(Camera mainCamera, Vector3 mousePosition)
    {
        mousePosition.z = 70f;
        
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    public static float GetAngleToMousePosition( Vector3 fromPosition, Vector3 toMousePosition)
    {
        Vector3 direction = (toMousePosition - fromPosition).normalized;

        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
