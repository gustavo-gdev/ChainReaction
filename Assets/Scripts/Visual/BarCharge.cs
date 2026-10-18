using UnityEngine;

public class BarCharge : MonoBehaviour{
    
    public Transform sizeObj;
    
    [SerializeField]
    private float _valueCurrent;
    public float ValueCurrent{
        get{
            return _valueCurrent;
        }
        set{
            _valueCurrent = Mathf.Clamp(value, 0, _valueMax);
        }
    }
    
    [SerializeField]
    private float _valueMax;
    public float ValueMax{
        get{
            return _valueMax;
        }
        set{
            _valueMax = value;
        }
    }
    
    // ====== ======
    
    public float ValueCurrentPerc{
        get{
            return _valueCurrent / _valueMax;
        }
    }
    
    public float SizeCurrent{
        get{
            return sizeMax * ValueCurrentPerc;
        }
    }
    public Vector2 SizeCurrentVector{
        get{
            return horizontalBar ? 
                new Vector2(SizeCurrent, sizeObj.localScale.y) : 
                new Vector2(sizeObj.localScale.x, SizeCurrent);
        }
    }
    
    public float sizeMax;
    public Vector2 SizeMaxVector{
        get{
            return horizontalBar ? 
                new Vector2(sizeMax, sizeObj.localScale.y) : 
                new Vector2(sizeObj.localScale.x, sizeMax);
        }
    }
    
    public bool horizontalBar = true;
    
    void Awake(){
        
        sizeMax = horizontalBar ? 
            sizeObj.localScale.x : 
            sizeObj.localScale.y;
    }
    
    // ====== ====== ====== ======
    
    public void UpdateBar(float barValue){
        
        ValueCurrent = barValue;
        UpdateBarSize();
    }
    
    public void UpdateBarSize(){
        
        sizeObj.localScale = (Vector3)SizeCurrentVector;
    }
}
