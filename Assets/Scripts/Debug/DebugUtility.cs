using UnityEngine;

public class DebugUtility : MonoBehaviour{
    
    public static void DrawBox(Vector2 positionCenter, Vector2 boxSize, Color boxColor){
        
        Vector2 topLeft = positionCenter + new Vector2(-boxSize.x, boxSize.y);
        Vector2 topRight = positionCenter + new Vector2(boxSize.x, boxSize.y);
        Vector2 bottomLeft = positionCenter + new Vector2(-boxSize.x, -boxSize.y);
        Vector2 bottomRight = positionCenter + new Vector2(boxSize.x, -boxSize.y);
        
        Debug.DrawLine(topLeft, topRight, boxColor);
        Debug.DrawLine(topRight, bottomRight, boxColor);
        Debug.DrawLine(bottomRight, bottomLeft, boxColor);
        Debug.DrawLine(bottomLeft, topLeft, boxColor);
    }
}
