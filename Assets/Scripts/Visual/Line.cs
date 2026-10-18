using UnityEngine;

public class Line : MonoBehaviour{
    
    public Transform linePivot;
    public Transform lineTip;
    
    [Space]
    
    public Vector2 lineDirection = Vector2.right;
    public float lineSizeInUnits = 1;
    public float lineSizeAdd;
    public float LineSize{
        get{
            return Vector2.Distance(Vector2.zero, lineDirection * lineSizeInUnits) + lineSizeAdd;
        }
    }
    
    void Update(){
        
        linePivot.rotation = Quaternion.FromToRotation(Vector2.right, lineDirection);
        linePivot.localScale = new Vector2(LineSize, linePivot.localScale.y);
        
        lineTip.position = (Vector2)linePivot.position + lineDirection.normalized * LineSize;
    }
    
    public void OnInstantiate(Vector2 lineDirection, float lineSizeInUnits){
        
        this.lineDirection = lineDirection;
        this.lineSizeInUnits = lineSizeInUnits;
    }
}
