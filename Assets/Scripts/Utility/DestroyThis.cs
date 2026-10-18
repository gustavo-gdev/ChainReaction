using UnityEngine;

public class DestroyThis : MonoBehaviour{
    
    public float destroyTime;
    
    void Start(){
        
        Destroy(gameObject, destroyTime);
    }
}
