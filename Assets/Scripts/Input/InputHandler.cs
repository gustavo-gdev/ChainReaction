using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour{
    
    private Camera _cameraMain;
    private LevelManager _levelManager;
    
    public InputActionReference mouseLeftAction;
    public InputActionReference mouseRightAction;
    
    private bool _mouseLeftDown;
    private bool _mouseRightDown;
    
    public bool MouseLeftHold{
        get{
            return mouseLeftHoldTimer >= buttonHoldLimit;
        }
    }
    public bool MouseRightHold{
        get{
            return mouseRightHoldTimer >= buttonHoldLimit;
        }
    }
    
    public float mouseLeftHoldTimer;
    public float mouseRightHoldTimer;
    
    public float buttonHoldLimit;
    
    [Space]
    
    private bool _inputEnabled;
    
    [Space]
    
    public GameObject objectOnMouseLeftDown;
    
    [Space]
    
    public float mouseLeftLastTime;
    public float mouseRightLastTime;
    
    public float clickDelay;
    
    public GameObject hoverObj;
    
    public event Action OnMouseClickLeftDown;
    public event Action OnMouseClickLeftUp;
    
    void Awake(){
        
        _cameraMain = Camera.main;
        _levelManager = FindObjectOfType<LevelManager>();
        _inputEnabled = true;
    }
    
    void OnEnable(){
        
        mouseLeftAction.action.performed += MouseLeftDown;
        mouseRightAction.action.performed += MouseRightDown;
        mouseLeftAction.action.canceled += MouseLeftUp;
        mouseRightAction.action.canceled += MouseRightUp;
        
        FindObjectOfType<GameOver>().OnGameOver += OnGameOver;
    }

    void OnDisable(){
        
        mouseLeftAction.action.performed -= MouseLeftDown;
        mouseRightAction.action.performed -= MouseRightDown;
        mouseLeftAction.action.canceled -= MouseLeftUp;
        mouseRightAction.action.canceled -= MouseRightUp;
        
        if(FindObjectOfType<GameOver>() != null){
            
            FindObjectOfType<GameOver>().OnGameOver -= OnGameOver;
        }
    }
    
    void Update(){
        
        MouseHoverCell();
        
        ButtonHold();
    }
    
    public void MouseHoverCell(){
        
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(_cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue()));
        
        GameObject hoverObjOld = hoverObj;
        
        if(!rayHit.collider){
            
            HoverCancel();
            return;
        }
        
        if(rayHit.collider.attachedRigidbody.GetComponent<IInputMouseObject>() == null){
            
            HoverCancel();
            return;
        }
        else{
            
            if(hoverObj == rayHit.collider.gameObject) return;
            
            hoverObj = rayHit.collider.attachedRigidbody.gameObject;
        }
        
        HoverCancel(false);
        
        hoverObj.GetComponent<IInputMouseObject>().OnMouseHover();
        
        void HoverCancel(bool hoverObjNull = true){
            
            if(hoverObjOld == null) return;
            
            hoverObjOld.GetComponent<IInputMouseObject>().OnMouseHoverOut();
            
            if(hoverObjNull) hoverObj = null;
        }
    }

    public void MouseLeftDown(InputAction.CallbackContext context){
        
        _mouseLeftDown = true;
        
        if(!context.performed) return;
        
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(_cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue()));
        
        if(!rayHit.collider) return;
        
        IInputMouseObject inputObj = rayHit.collider.attachedRigidbody.GetComponent<IInputMouseObject>();
        
        if(inputObj == null) return;
        
        if(!InputGameplayEnabled && rayHit.collider.attachedRigidbody.GetComponent<Cell>() != null) return;
        
        inputObj.OnMouseClickLeftDown();
        objectOnMouseLeftDown = rayHit.collider.attachedRigidbody.gameObject;
    }
    
    public void MouseRightDown(InputAction.CallbackContext context){
        
        if(!InputGameplayEnabled) return;
        
        _mouseRightDown = true;
    }

    public void MouseLeftUp(InputAction.CallbackContext context){
        
        _mouseLeftDown = false;
        
        float lastTime = (float)context.time - mouseLeftLastTime;
        mouseLeftLastTime = (float)context.time;
        
        if(lastTime <= clickDelay){
            
            return;
        }
        
        
        if(context.canceled){
            
            if(objectOnMouseLeftDown != null){
                
                if(InputGameplayEnabled || objectOnMouseLeftDown.GetComponent<Cell>() == null){
                    
                    objectOnMouseLeftDown.GetComponent<IInputMouseObject>().OnMouseClickLeftUp();
                }
                
                objectOnMouseLeftDown = null;
            }
        }
        
        OnMouseClickLeftUp?.Invoke();
    }

    public void MouseRightUp(InputAction.CallbackContext context){
        
        if(!InputGameplayEnabled) return;
        
        _mouseRightDown = false;
        
        float lastTime = (float)context.time - mouseRightLastTime;
        mouseRightLastTime = (float)context.time;
        
        if(lastTime <= clickDelay){
            
            return;
        }
        if(!context.canceled) return;
        
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(_cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue()));
        
        if(!rayHit.collider) return;
        
        IInputMouseObject inputObj = rayHit.collider.attachedRigidbody.GetComponent<IInputMouseObject>();
        
        if(inputObj == null) return;
        
        inputObj.OnMouseClickRightUp();
    }
    
    public Cell CellUnderCursor(){
        
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(_cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue()));
        
        if(!rayHit.collider || rayHit.collider.GetComponent<Cell>() == null) return null;
        
        return rayHit.collider.GetComponent<Cell>();
    }
    
    // ====== ======
    
    public void ButtonHold(){
        
        if(_mouseLeftDown){
            
            mouseLeftHoldTimer += Time.deltaTime;
        }
        else{
            
            mouseLeftHoldTimer = 0;
        }
        
        if(_mouseRightDown){
            
            mouseRightHoldTimer += Time.deltaTime;
        }
        else{
            
            mouseRightHoldTimer = 0;
        }
    }
    
    public bool InputGameplayEnabled{
        get{
            if(_levelManager.Swaping) return false;
            if(_levelManager.CellFalling) return false;
            if(_levelManager.CellExplodingQueue) return false;
            if(GameOver.gameOver) return false;
            if(HelpScreen.helpScreen) return false;
            return true;
        }
    }
    
    public void InputDisable(){
        
        _inputEnabled = false;
    }
    
    public void InputEnable(){
        
        _inputEnabled = true;
    }
    
    // ====== ====== ====== ======
    
    public void OnGameOver(){
        
        objectOnMouseLeftDown = null;
        hoverObj = null;
    }
}
