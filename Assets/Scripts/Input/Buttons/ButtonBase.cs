using UnityEngine;

public class ButtonBase : MonoBehaviour, IInputMouseObject{
    
    public GameObject overlayObj;
    private Animator _animator;
    
    public string animOverlayInactive;
    public string animOverlayHover;
    
    public virtual void Awake(){
        
        if(overlayObj != null){
            
            _animator = overlayObj.GetComponent<Animator>();
        }
    }
    
    public virtual void OnMouseClickLeftDown(){ }

    public virtual void OnMouseClickLeftUp(){ }

    public virtual void OnMouseClickRightDown(){ }

    public virtual void OnMouseClickRightUp(){ }
    
    public void OnMouseHover(){
        
        AnimationOverlayUpdate(animOverlayHover);
    }

    public void OnMouseHoverOut(){
        
        AnimationOverlayUpdate(animOverlayInactive);
    }
    
    public void AnimationOverlayUpdate(string animationName){
        
        if(_animator != null){
            
            _animator.Play(animationName);
        }
    }
}
