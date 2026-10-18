public class ButtonReset : ButtonBase{

    public override void OnMouseClickLeftUp(){
        
        base.OnMouseClickLeftUp();
        
        FindObjectOfType<LevelManager>().ResetGame();
    }
}
