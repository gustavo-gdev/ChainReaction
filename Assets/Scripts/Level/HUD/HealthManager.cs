using System;
using UnityEngine;

public class HealthManager : MonoBehaviour{
    
    public int health;
    private static int _healthMax = 3;
    
    public event Action OnHealthUpdate;
    
    public void Damage(int damage = 1){
        
        health -= damage;
        
        OnHealthUpdate?.Invoke();
    }
    
    public void HealthReset(){
        
        health = _healthMax;
    }
}
