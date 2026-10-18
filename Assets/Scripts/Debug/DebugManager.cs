using UnityEngine;
using dutil = DebugUtility;

public class DebugManager : MonoBehaviour{
    
    private LevelManager _levelManager;
    
    public bool debugActive;
    
    [Space]
    
    public bool showDebugMines;
    public bool showDebugEmpty;
    
    public float revealTimerMultiplyer;
    
    public BoardGrid GridCurrent{
        get{
            return _levelManager.gridCurrent;
        }
    }
    
    void Awake(){
        
        _levelManager = FindObjectOfType<LevelManager>();
        
        // PlayerPrefs.SetInt("HiScoreNormal", 150000);
        // PlayerPrefs.SetInt("HiScoreHard", 200000);
        // PlayerPrefs.SetInt("HiScoreExpert", 250000);
        
        // PlayerPrefs.SetInt("HiScoreNormal", 0);
        // PlayerPrefs.SetInt("HiScoreHard", 0);
        // PlayerPrefs.SetInt("HiScoreExpert", 0);
    }
    
    void Update(){
        
        if(!debugActive) return;
        
        if(showDebugMines){
            
            foreach(CellMine mines in GridCurrent.Mines()){
                
                dutil.DrawBox(mines.transform.position, .45f * GameValues.CellSize * Vector2.one, Color.red);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.R)){
            
            GameData.ResetHiScore();
        }
        
        revealTimerMultiplyer = FindObjectOfType<CellRevealOvertime>().RevealTimerMultiplyer;
        
        if(Input.GetKeyDown(KeyCode.D)){
            
            Debug.Log("====== Hi-Score ======");
            Debug.Log($"Normal {PlayerPrefs.GetInt("HiScoreNormal")}");
            Debug.Log($"Hard {PlayerPrefs.GetInt("HiScoreHard")}");
            Debug.Log($"Expert {PlayerPrefs.GetInt("HiScoreExpert")}");
        }
    }
}
