public class ButtonHelp : ButtonBase{

    public override void OnMouseClickLeftUp(){
        
        base.OnMouseClickLeftUp();
        
        FindObjectOfType<HelpScreen>().HelpScreenToggle();
    }
}