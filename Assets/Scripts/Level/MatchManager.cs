using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour{
    
    public static int matchCountMin = 3;
    
    public void MatchCheck(CellMine mineCheck){
        
        MatchCardinalCheck(mineCheck, Vector2.right);
        MatchCardinalCheck(mineCheck, Vector2.up);
        MatchDiagonalCheck(mineCheck, Vector2.one);
        MatchDiagonalCheck(mineCheck, new Vector2(1, -1));
    }
    
    public void MatchCardinalCheck(CellMine mineCheck, Vector2 direction){
        
        int distance = 1;
        
        List<CellMine> matchMines = new(){ mineCheck };
            
        CellMine mineDirection;
        
        
        // POSITIVE DIRECTION
        
        do{
            
            mineDirection = GameUtility.MineDirection(mineCheck.transform.position, direction, distance);
            
            if(mineDirection != null && mineDirection.marked){
                
                matchMines.Add(mineDirection);
            }
            
            distance ++;
        }
        while (mineDirection != null && mineDirection.marked);
        
        distance = 1;
        
        
        // NEGATIVE DIRECTION
        
        do{
            
            mineDirection = GameUtility.MineDirection(mineCheck.transform.position, -direction, distance);
            
            if(mineDirection != null && mineDirection.marked){
                
                matchMines.Add(mineDirection);
            }
            
            distance ++;
        }
        while (mineDirection != null && mineDirection.marked);
        
        if(matchMines.Count >= matchCountMin){
            
            foreach (CellMine matchMine in matchMines){
                
                matchMine.Explosion();
            }
        }
    }
    
    
    public void MatchDiagonalCheck(CellMine mineCheck, Vector2 direction){
        
        int distance = 1;
        
        List<CellMine> matchMines = new(){ mineCheck };
            
        CellMine mineDirection;
        
        
        // POSITIVE DIRECTION
        
        do{
            
            mineDirection = GameUtility.MineDirection(mineCheck.transform.position + new Vector3(0, direction.y * distance), new Vector2(direction.x, 0), distance);
            
            if(mineDirection != null && mineDirection.marked){
                
                matchMines.Add(mineDirection);
            }
            
            distance ++;
        }
        while (mineDirection != null && mineDirection.marked);
        
        distance = 1;
        
        
        // NEGATIVE DIRECTION
        
        do{
            
            mineDirection = GameUtility.MineDirection(mineCheck.transform.position + new Vector3(0, -direction.y * distance), new Vector2(-direction.x, 0), distance);
            
            if(mineDirection != null && mineDirection.marked){
                
                matchMines.Add(mineDirection);
            }
            
            distance ++;
        }
        while (mineDirection != null && mineDirection.marked);
        
        if(matchMines.Count >= matchCountMin){
            
            foreach (CellMine matchMine in matchMines){
                
                matchMine.Explosion();
            }
        }
    }
}
