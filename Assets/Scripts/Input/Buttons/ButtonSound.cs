using UnityEngine;

public class ButtonSound : ButtonBase{
    
    public GameObject soundOffOverlay;
    
    public override void Awake(){
        
        base.Awake();
        
        soundOffOverlay.SetActive(!GameData.SettingSound);
    }
    
    public override void OnMouseClickLeftUp(){
        
        base.OnMouseClickLeftUp();
        
        GameData.ToggleSound();
        
        soundOffOverlay.SetActive(!GameData.SettingSound);
    }
}