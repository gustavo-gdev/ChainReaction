using UnityEngine;

public class Logo : MonoBehaviour, IInputMouseObject{
    
    private bool _hovering;

    public void OnMouseClickLeftDown(){
    }

    public void OnMouseClickLeftUp(){
    }

    public void OnMouseClickRightDown(){
    }

    public void OnMouseClickRightUp(){
    }

    public void OnMouseHover(){
        
        if(!_hovering){
            
            _hovering = true;
            
            GetComponent<Animator>().Play("Logo_Hover");
        }
    }

    public void OnMouseHoverOut(){
        
        if(_hovering){
            
            _hovering = false;
        }
    }
}
