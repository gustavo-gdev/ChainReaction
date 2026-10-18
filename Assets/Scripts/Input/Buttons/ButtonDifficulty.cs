using UnityEngine;
using TMPro;

public class ButtonDifficulty : ButtonBase{
    
    public GameDifficulty difficulty;
    
    [Space]
    
    private bool _enabledButton;
    public GameObject enabledObj;
    
    [Space]
    
    public TextMeshPro buttonText;
    public Color ColorDisabled{
        get{
            return new Color(0.247f, 0.247f, 0.247f);
        }
    }
    public Color ColorEnabled{
        get{
            return new Color(0f, 0.184f, 0.384f);
        }
    }
    
    void Start(){
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        if(difficulty == levelManager.Difficulty){
        
            EnableButton();
        }
        
        if(!_enabledButton){
            
            DisableButton();
        }
    }
    
    public override void OnMouseClickLeftUp(){
        
        EnableButton();
    }
    
    public void EnableButton(){
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        if(difficulty == levelManager.DifficultyOnReset || _enabledButton) return;
        
        _enabledButton = true;
        
        ButtonDifficulty buttonEnabledOld = null;
        ButtonDifficulty[] buttons = FindObjectsOfType<ButtonDifficulty>();
        
        foreach(ButtonDifficulty button in buttons){
            
            if(button.difficulty == levelManager.DifficultyOnReset){
                
                buttonEnabledOld = button;
                break;
            }
        }
        
        if(buttonEnabledOld != null && buttonEnabledOld != this){
            
            buttonEnabledOld.DisableButton();
        }
        
        enabledObj.SetActive(true);
        buttonText.color = ColorEnabled;
        
        if(!levelManager.GameStarted){
            
            levelManager.Difficulty = difficulty;
            
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            
            scoreManager.SetHiScore(GameData.GetHiScore());
            scoreManager.HiScoreUpdate();
        }
        
        levelManager.DifficultyOnReset = difficulty;
    }
    
    public void DisableButton(){
        
        _enabledButton = false;
        enabledObj.SetActive(false);
        buttonText.color = ColorDisabled;
    }
}
