using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeLimit : MonoBehaviour{
    
    private LevelManager _levelManager;
    
    public event Action OnDamageTimer;
    public event Action OnAddTimer;
    
    [SerializeField]
    private float _timeLimitTimer;
    public float TimeLimitTimer{
        get{
            return _timeLimitTimer;
        }
        set{
            _timeLimitTimer = Mathf.Clamp(value, 0, timeLimitMax);
        }
    }
    
    [Space]
    
    public float timeLimitMax;
    private Dictionary<GameDifficulty, float> timeLimitMaxDifficulty = new();
    
    public float TimerPerc{
        get{
            return TimeLimitTimer / timeLimitMax;
        }
    }
    
    public float TimerLowPerc{
        get{
            return .2f;
        }
    }
    
    public bool UnderTimerLowerPerc{
        get{
            return TimerPerc < TimerLowPerc;
        }
    }
    
    public bool gameOver;
    
    void Awake(){
        
        _levelManager = FindObjectOfType<LevelManager>();
        
        TimerReset();
        
        timeLimitMaxDifficulty.Add(GameDifficulty.Normal, 120);
        timeLimitMaxDifficulty.Add(GameDifficulty.Hard, 90);
        timeLimitMaxDifficulty.Add(GameDifficulty.Expert, 60);
    }
    
    void Update(){
        
        TimerUpdate();
    }
    
    public void TimerUpdate(){
        
        if(!CanTimerUpdate) return;
        
        TimeLimitTimer -= Time.deltaTime;
        
        GameOverCheck();
    }
    
    public void GameOverCheck(){
        
        if(TimeLimitTimer <= 0){
            
            TimeLimitTimer = 0;
            
            gameOver = true;
            
            FindObjectOfType<GameOver>().GameOverStart(GameOver.GameOverType.Time);
        }
    }
    
    public void SetTimeLimitMax(GameDifficulty gameDifficulty){
        
        timeLimitMax = timeLimitMaxDifficulty[gameDifficulty];
        TimeLimitTimer = timeLimitMax;
        
    }
    
    public void TimerReset(){
        
        TimeLimitTimer = timeLimitMax;
        gameOver = false;
    }
    
    public bool CanTimerUpdate{
        get{
            if(_levelManager.CellFalling) return false;
            if(_levelManager.CellExplodingQueue) return false;
            if(!_levelManager.GameStarted) return false;
            if(GameOver.gameOver) return false;
            if(gameOver) return false;
            return true;
        }
    }
    
    
    // ====== ====== ====== ======
    
    
    public float DefaultAddTime{
        get{
            return 10f;
        }
    }
    
    public void AddTimer(float addTime = 0){
        
        if(addTime <= 0) addTime = DefaultAddTime;
        
        float DifficultyMultiplyer(){
            
            GameDifficulty gameDifficulty = FindObjectOfType<LevelManager>().Difficulty;
            return gameDifficulty switch
            {
                GameDifficulty.Normal => 1f,
                GameDifficulty.Hard => .8f,
                GameDifficulty.Expert => .6f,
                _ => 1f,
            };
        }
        
        TimeLimitTimer += addTime * DifficultyMultiplyer();
        
        OnAddTimer?.Invoke();
    }
    
    
    // ====== ====== ====== ======
    
    
    private float DamageValuePerc{
        get{
            return .14f;
        }
    }
    
    public void DamageTimer(){
        
        OnDamageTimer?.Invoke();
        TimeLimitTimer -= DamageValuePerc * timeLimitMax;
        
        GameOverCheck();
    }
}
