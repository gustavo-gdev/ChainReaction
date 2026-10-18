using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudManager : MonoBehaviour{
    
    private ScoreManager _scoreManager;
    private GameTimeLimit _timeLimit;
    
    [Space]
    
    public TextMeshPro scoreText;
    public TextMeshPro hiScoreText;
    
    [Space]
    
    public Image[] healthSprites;
    
    [Space]
    
    public TextMeshPro timeLimitText;
    public BarCharge timeBar;
    
    [Space]
    
    public Transform damageTimerMaskObj;
    public Transform damageTimerObj;
    
    [Space]
    
    public Transform healTimerMaskObj;
    public Transform healTimerObj;
    
    [Space]
    
    public GameObject timerLowObj;
    public bool timerLow;
    
    [Space]
    
    public GameObject difficultyChangeWarning;
    
    void Awake(){
        
        _scoreManager = FindObjectOfType<ScoreManager>();
        _timeLimit = FindObjectOfType<GameTimeLimit>();
        
        UpdateTimeBar();
    }

    void OnEnable(){
        
        _scoreManager.OnScoresUpdate += ScoreUpdate;
        _scoreManager.OnHiScoresUpdate += HiScoreUpdate;
        _timeLimit.OnDamageTimer += HealthDamageUpdate;
        _timeLimit.OnAddTimer += HealthHealUpdate;
    }
    
    void OnDisable(){
        
        _scoreManager.OnScoresUpdate -= ScoreUpdate;
        _scoreManager.OnHiScoresUpdate -= HiScoreUpdate;
        _timeLimit.OnDamageTimer -= HealthDamageUpdate;
        _timeLimit.OnAddTimer -= HealthHealUpdate;
    }
    
    void Update(){
        
        TimeLimitUpdate();
    }
    
    // ====== ====== ======= ======
    
    public void ScoreUpdate(){
        
        scoreText.text = _scoreManager.scoreCurrent + "";
    }
    
    public void HiScoreUpdate(){
        
        hiScoreText.text = _scoreManager.hiScoreCurrent + "";
    }
    
    
    // ====== ====== ======= ======
    
    
    public void HealthHealUpdate(){
        
        TimeLimitUpdate(true);
    }
    
    public void HealthDamageUpdate(){
        
        damageTimerMaskObj.transform.position = timeBar.sizeObj.position;
        damageTimerMaskObj.transform.localScale = timeBar.sizeObj.localScale;
        
        damageTimerObj.GetComponent<Animator>().Play("DamageTimer_Damage");
    }
    
    
    // ====== ====== ======= ======
    
    
    public void TimeLimitUpdate(bool heal = false){
        
        timeLimitText.text = Mathf.CeilToInt(_timeLimit.TimeLimitTimer) + "";
        
        timeBar.UpdateBar(_timeLimit.TimeLimitTimer);
        
        if(_timeLimit.UnderTimerLowerPerc && !timerLow){
            
            timerLowObj.GetComponent<Animator>().Play("TimerLow_Warning");
            timerLow = true;
        }
        else if(!_timeLimit.UnderTimerLowerPerc && timerLow){
            
            timerLowObj.GetComponent<Animator>().Play("TimerLow_None");
            timerLow = false;
        }
        
        if(heal){
            
            healTimerMaskObj.transform.position = timeBar.sizeObj.position;
            healTimerMaskObj.transform.localScale = timeBar.sizeObj.localScale;
            
            healTimerObj.GetComponent<Animator>().Play("DamageTimer_Damage");
        }
    }
    
    public void DifficultyChangeWarning(bool activeObj){
        
        difficultyChangeWarning.SetActive(activeObj);
    }
    
    public void UpdateTimeBar(){
        
        timeBar.ValueMax = _timeLimit.timeLimitMax;
    }
}
