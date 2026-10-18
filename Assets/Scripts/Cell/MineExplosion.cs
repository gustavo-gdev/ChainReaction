using UnityEngine;

public class MineExplosion : MonoBehaviour{
    
    public void StartExplosion(int explosionRange, bool cardinal){
        
        string animExplosion = !cardinal ? "Explosion_Range3" : 
            (explosionRange == 1) ? "Explosion_Range1" : "Explosion_Range2";
        
        GetComponent<Animator>().Play(animExplosion);
    }
    
    public void DestroyThis(){
        
        Destroy(gameObject);
    }
}
