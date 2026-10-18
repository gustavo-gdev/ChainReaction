using System.Collections.Generic;
using UnityEngine;

public class GameUtility : MonoBehaviour{
    
    public static Vector2[] SuffleListVector2(Vector2[] listShuffle){
        
        List<Vector2> newList = new();
        
        foreach (Vector2 item in listShuffle){
            
            int randomIndex = Random.Range(0, newList.Count);
            newList.Insert(randomIndex, item);
        }
        
        return newList.ToArray();
    }
    
    public static int[] ShuffleListInt(int[] listShuffle){
        
        List<int> newList = new();
        
        foreach (int item in listShuffle){
            
            int randomIndex = Random.Range(0, newList.Count);
            newList.Insert(randomIndex, item);
        }
        
        return newList.ToArray();
    }
        
    public static Vector2 CameraSizeWorld{
        
        get{
            float aspect = (float)Screen.width / Screen.height;	
            float _camHeight = Camera.main.orthographicSize * 2;
            
            return new Vector2(_camHeight * aspect, _camHeight);
        }
    }
    
    public static Cell CellDirection(Vector2 pivot, Vector2 direction, float distance){
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        RaycastHit2D rayHitCell = Physics2D.Raycast(pivot + direction * distance * GameValues.CellSize, Vector2.right, + GameValues.CellSize * .25f, levelManager.cellLayer);
        
        if(rayHitCell.collider == null) return null;
        
        Cell cell = rayHitCell.collider.GetComponent<Cell>();
        
        return cell;
    }
    
    public static CellMine MineDirection(Vector2 pivot, Vector2 direction, float distance){
        
        Cell cell = CellDirection(pivot, direction, distance);
        
        return cell != null ? CellDirection(pivot, direction, distance).GetComponent<CellMine>() : null;
    }
}
