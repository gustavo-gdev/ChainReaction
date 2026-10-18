using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineBackground : MonoBehaviour{
    
    public LineRenderer lineRenderer;
    
    public float positionStartPoint;
    
    public float moveSpeed;
    
    public Vector3 MoveDirection{
        get{
            return Vector2.left;
        }
    }
    
    void Awake(){
        
        positionStartPoint = lineRenderer.GetPosition(0).x;
    }
    
    void Update(){
        
        MoveLine();
    }
    
    // ====== ====== ====== ======
    
    public void MoveLine(){
        
        for (int i = 0; i < lineRenderer.positionCount; i++){
            
            lineRenderer.SetPosition(i, lineRenderer.GetPosition(i) + MoveSpeed);
        }
        
        if(lineRenderer.GetPosition(0).x < positionStartPoint){
            
            List<Vector3> points = new();
            
            for (int i = 0; i < lineRenderer.positionCount; i++){
                
                points.Add(lineRenderer.GetPosition(i));
            }
            
            points.Add(new Vector3(points.Last().x + PointsDistanceX, points[0].y));
            points.RemoveAt(0);
            
            lineRenderer.SetPositions(points.ToArray());
        }
    }
    
    // ====== ====== ====== ======
    
    public Vector3 PointPosition0{
        get{
            if(lineRenderer == null) return Vector2.zero;
            return lineRenderer.GetPosition(0);
        }
    }
    
    public Vector3 PointPosition1{
        get{
            if(lineRenderer == null) return Vector2.zero;
            return lineRenderer.GetPosition(1);
        }
    }
    
    public float PointsDistanceX{
        get{
            return Mathf.Abs(PointPosition0.x - PointPosition1.x);
        }
    }
    
    // ====== ======
    
    public Vector3 MoveSpeed{
        get{
            return moveSpeed * Time.deltaTime * MoveDirection;
        }
    }
}
