using UnityEngine;

public static class CalcUtility{
    
    public static bool PointInVectorRange(Vector2 point, Vector2 rangePosiiton, Vector2 vectorRange){
        
        return Mathf.Abs(point.x - rangePosiiton.x) < vectorRange.x && Mathf.Abs(point.y - rangePosiiton.y) < vectorRange.y;
    }
    
    public static Vector2 Vector2Abs(Vector2 vector2){
        
        return new Vector2(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y));
    }
    
    public static Vector2 Vector2ToInt(Vector2 vector2){
        
        return new Vector2((int)vector2.x, (int)vector2.y);
    }
}