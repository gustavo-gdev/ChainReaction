using UnityEngine;

public class GameData : MonoBehaviour{
    
    private static bool _settingSound = true;
    public static bool SettingSound{
        get{
            return _settingSound;
        }
    }
    
    void Awake(){
        
        _settingSound = PlayerPrefs.GetInt("SettingSound") == 1;
    }
    
    public static void SaveHiScore(int hiScore){
        
        switch (FindObjectOfType<LevelManager>().Difficulty){
            
            case GameDifficulty.Normal:
                
                PlayerPrefs.SetInt("HiScoreNormal", hiScore);
                break;
            
            case GameDifficulty.Hard:
                
                PlayerPrefs.SetInt("HiScoreHard", hiScore);
                break;
            
            case GameDifficulty.Expert:
                
                PlayerPrefs.SetInt("HiScoreExpert", hiScore);
                break;
                
            default:
                break;
        }
    }
    
    public static int GetHiScore(){
        
        return FindObjectOfType<LevelManager>().Difficulty switch
        {
            GameDifficulty.Normal => PlayerPrefs.GetInt("HiScoreNormal"),
            GameDifficulty.Hard => PlayerPrefs.GetInt("HiScoreHard"),
            GameDifficulty.Expert => PlayerPrefs.GetInt("HiScoreExpert"),
            _ => 0,
        };
    }
    
    public static void ToggleSound(){
        
        _settingSound = !_settingSound;
        SaveSettings();
    }
    
    public static void SaveSettings(){
        
        PlayerPrefs.SetInt("SettingSound", _settingSound ? 1 : 0);
    }
    
    // ======
    
    public static void ResetHiScore(){
        
        PlayerPrefs.SetInt("HiScoreNormal", 0);
        PlayerPrefs.SetInt("HiScoreHard", 0);
        PlayerPrefs.SetInt("HiScoreExpert", 0);
        
        FindObjectOfType<ScoreManager>().hiScoreCurrent = GetHiScore();
        
        FindObjectOfType<ScoreManager>().HiScoreUpdate();
    }
}
