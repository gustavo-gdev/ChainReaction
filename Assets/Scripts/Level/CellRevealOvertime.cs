using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellRevealOvertime : MonoBehaviour{
    
    private LevelManager _levelManager;
    private BoardGrid GridCurrent{
        get{
            return _levelManager.gridCurrent;
        }
    }
    
    [SerializeField]
    private float _revealTimer;
    public float revealLimit;
    
    public float revealTimerReset;
    
    [Space]
    
    public float revealTimerMultiplyerMin = 1;
    public float revealTimerMultiplyerMax = 3;
    
    public float RevealTimerMultiplyer{
        get{
            int cellsRevealedCounter = _levelManager.gridCurrent.CellsRevealed().Length;
            int cellsCountMin = GameValues.BoardCellsCount;
            
            float multiplyerPerc = 1 - (float)cellsRevealedCounter / cellsCountMin;
            
            return (revealTimerMultiplyerMax - revealTimerMultiplyerMin) * multiplyerPerc + revealTimerMultiplyerMin;
        }
    }
    
    [Space]
    
    public int revealedCount;
    
    void Awake(){
        
        _levelManager = FindObjectOfType<LevelManager>();
    }
    
    void Update(){
        
        ReavealUpdate();
    }
    
    public void ReavealUpdate(){
        
        if(!CanReveal) return;
        
        _revealTimer += Time.deltaTime * RevealTimerMultiplyer;
        
        if(_revealTimer >= revealLimit){
            
            CellEmpty cellRandom = CellRandom();
            RevealCell(cellRandom);
            
            ResetTimer();
        }
    }
    
    
    public void RevealCell(CellEmpty cell){
        
        if(cell == null) return;
        
        cell.ActivateCell(false);
    }
    
    
    public CellEmpty CellRandom(){
        
        CellEmpty[] cells = GridCurrent.EmptyCells().Where(value => !value.activated).ToArray();
        int randomIndex = Random.Range(0, cells.Length);
        
        return (randomIndex == 0) ? null : cells[randomIndex];
    }
    
    public void ResetTimer(){
        
        if(_revealTimer >= revealLimit){
            
            revealedCount++;
            int revealedCountMultiplyer = (revealedCount > 5) ? 5 : revealedCount;
            
            float revealLimitFraction = revealLimit * .1f;
            
            _revealTimer = revealLimitFraction * revealedCountMultiplyer;
        }
        else if(_revealTimer >= revealTimerReset){
            
            _revealTimer = 0;
        }
    }
    
    public bool CanReveal{
        get{
            if(_levelManager.CellFalling) return false;
            if(_levelManager.CellExplodingQueue) return false;
            if(_levelManager.gridCurrent.regenerateOnClick) return false;
            if(GameOver.gameOver) return false;
            return true;
        }
    }
}
