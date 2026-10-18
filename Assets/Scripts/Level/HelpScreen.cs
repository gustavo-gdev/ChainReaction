using UnityEngine;

public class HelpScreen : MonoBehaviour{
    
    public static bool helpScreen;
    
    public GameObject helpScreenObj;
    
    public string helpScreenAnimationStart;
    public string helpScreenAnimationEnd;
    
    void Awake(){
        
        helpScreen = false;
    }
    
    public void HelpScreenToggle(){
        
        switch (helpScreen){
            
            case false:
                
                HelpScreenStart();
                break;
            
            case true:
                
                HelpScreenEnd();
                break;
        }
    }
    
    public void HelpScreenStart(){
        
        if(helpScreen) return;
        
        helpScreenObj.GetComponent<Animator>().Play(helpScreenAnimationStart);
        
        helpScreen = true;
    }
    
    public void HelpScreenEnd(){
        
        if(!helpScreen) return;
        
        helpScreenObj.GetComponent<Animator>().Play(helpScreenAnimationEnd);
        
        helpScreen = false;
    }
}
