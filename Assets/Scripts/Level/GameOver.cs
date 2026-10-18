using UnityEngine;
using TMPro;
using System;

public class GameOver : MonoBehaviour{
    
    public static bool gameOver;
    
    [Space]
    
    public TextMeshPro scoreText;
    
    [Space]
    
    public TextMeshPro hiScoreText;
    public TextMeshPro newHiScoreText;
    
    [Space]
    
    public GameObject gameOverScreenObj;
    public string gameOverScreenAnimationTime;
    public string gameOverScreenAnimationMine;
    
    public string gameOverScreenAnimationEnd;
    
    public enum GameOverType{
        Time, 
        Mine
    }
    
    public event Action OnGameOver;
    
    void Awake(){
        
        gameOver = false;
    }
    
    public void GameOverStart(GameOverType gameOverType, Cell cellMine = null){
        
        if(gameOver) return;
        
        switch (gameOverType){
            
            case GameOverType.Time:
                
                gameOverScreenObj.GetComponent<Animator>().Play(gameOverScreenAnimationTime);
                
                break;
            
            case GameOverType.Mine:
                
                gameOverScreenObj.GetComponent<Animator>().Play(gameOverScreenAnimationMine);
                RevealMines(cellMine);
                
                break;
        }
        
        ScoreUpdate();
        
        gameOver = true;
        
        OnGameOver?.Invoke();
    }
    
    public void GameOverEnd(){
        
        if(!gameOver) return;
        
        gameOverScreenObj.GetComponent<Animator>().Play(gameOverScreenAnimationEnd);
        
        gameOver = false;
    }
    
    
    // ====== ====== ====== ======
    
    public void RevealMines(Cell cellMineGameOver){
        
        GameObject markFail = Instantiate(FindObjectOfType<GamePrefabs>().markFailPrefab, cellMineGameOver.transform.position, Quaternion.identity);
        
        markFail.GetComponent<DestroyThis>().destroyTime = 10;
        markFail.transform.parent = cellMineGameOver.transform;
        
        CellMine[] cellMines = FindObjectOfType<LevelManager>().gridCurrent.Mines();
        
        foreach(CellMine cellMine in cellMines){
            
            cellMine.AnimationUpdate(cellMine.animActivate);
        }
    }
    
    // ====== ====== ====== ======
    
    
    public void ScoreUpdate(){
        
        scoreText.text = FindObjectOfType<ScoreManager>().scoreCurrent + "";
        
        if(FindObjectOfType<ScoreManager>().newHiScore){
            
            newHiScoreText.gameObject.SetActive(true);
            hiScoreText.gameObject.SetActive(false);
            
            newHiScoreText.text = $"NEW HI-SCORE!!";
        }
        else{
            
            hiScoreText.gameObject.SetActive(true);
            newHiScoreText.gameObject.SetActive(false);
            
            hiScoreText.text = $"HI-SCORE: {GameData.GetHiScore()}";
        }
    }
}
