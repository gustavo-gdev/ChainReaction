using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour{
    
    public int scoreCurrent;
    public int hiScoreCurrent;
    public bool newHiScore;
    
    public Dictionary<GameDifficulty, int> hiScoresDifficulty = new();
    
    public static int baseScoreMultiplyer = 100;
    
    public event Action OnScoresUpdate;
    public event Action OnHiScoresUpdate;
    
    void Start(){
        
        hiScoreCurrent = GameData.GetHiScore();
        HiScoreUpdate();
    }

    void OnEnable(){
        
        FindObjectOfType<GameOver>().OnGameOver += NewHighScoreReset;
    }

    void OnDisable(){
        
        if(FindObjectOfType<GameOver>() != null){
            
            FindObjectOfType<GameOver>().OnGameOver -= NewHighScoreReset;
        }
    }
    
    public void AddScore(int scoreToAdd){
        
        scoreCurrent += scoreToAdd * baseScoreMultiplyer;
        
        ScoreUpdate();
        
        if(scoreCurrent > hiScoreCurrent){
            
            SetHiScore(scoreCurrent);
            
            HiScoreUpdate();
            newHiScore = true;
        }
        
        GameData.SaveHiScore(hiScoreCurrent);
    }
    
    
    public void SetScore(int score = 0){
        
        scoreCurrent = score;
        ScoreUpdate();
    }
    
    // ====== ====== ====== ======
    
    public void SetHiScore(int hiScore){
        
        hiScoreCurrent = hiScore;
        HiScoreUpdate();
    }
    
    
    // ====== ====== ====== ======
    
    public void ScoreUpdate(){
        
        OnScoresUpdate?.Invoke();
    }
    
    public void HiScoreUpdate(){
        
        OnHiScoresUpdate?.Invoke();
    }
    
    public void NewHighScoreReset(){
        
        newHiScore = false;
    }
}
