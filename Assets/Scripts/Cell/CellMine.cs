using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellMine : Cell{
    
    public bool exploded;
    public bool explodedQueue;
    
    [Space]
    
    [SerializeField]
    private int _explosionRange = 1;
    
    [Space]
    
    public float explosionDurationTimer;
    public static float explosionDurationLimit = .4f;
    
    public bool ExplosionTypeCardinal{
        get{
            return explosionType == ExplosionTypes.cardinal || explosionType == ExplosionTypes.adjacent;
        }
    }
    public bool ExplosionTypeDiagonal{
        get{
            return explosionType == ExplosionTypes.diagonal || explosionType == ExplosionTypes.adjacent;
        }
    }
    
    public ExplosionTypes explosionType;
    
    public enum ExplosionTypes{
        cardinal, 
        diagonal, 
        adjacent
    }
    
    public override bool CanDrag{ get { return true; } }
    
    [Space]
    
    public string animGameOver;
    public string animExploded;
    
    public string animMineCardinal;
    public string animMineDiagonal;
    public string animMineAdjacent;
    public string AnimMine{
        get{
            return explosionType switch
            {
                ExplosionTypes.cardinal => animMineCardinal,
                ExplosionTypes.diagonal => animMineDiagonal,
                ExplosionTypes.adjacent => animMineAdjacent,
                _ => animMineAdjacent,
            };
        }
    }

    // ====== ====== ====== ======
    
    public override void Awake(){
        
        base.Awake();
        
        ExplosionTypeRandomize();
    }
    
    public override void Update(){
        
        ExplosionUpdate();
    }
    
    
    // ====== ====== ====== ======
    
    
    public override void ActivateCell(bool addScore = true){
        
        base.ActivateCell();
        
        if(!marked){
            
            FindObjectOfType<AudioManager>().Play(SoundsNames.ExplosionDeath);
            
            FindObjectOfType<GameOver>().GameOverStart(GameOver.GameOverType.Mine, this);
            
            AnimationUpdate(animGameOver);
        }
    }

    public override void MarkCell(){
        
        base.MarkCell();
        
        if(marked) return;
        
        marked = true;
        
        FindObjectOfType<AudioManager>().Play(SoundsNames.Mine);
        
        AnimationUpdate(AnimMine);
        
        FindObjectOfType<CellRevealOvertime>().ResetTimer();
        FindObjectOfType<MatchManager>().MatchCheck(this);
    }
    
    public void Explosion(){
        
        if(exploded) return;
        
        int explosionRange = _explosionRange;
        
        List<Cell> cellsToDestroy = new();
        if(ExplosionTypeCardinal){
            
            cellsToDestroy.AddRange(GetCellsCardinal(explosionRange));
            
            GameObject explosion = Instantiate(FindObjectOfType<GamePrefabs>().explosionPrefab, transform.position, Quaternion.identity);
            explosion.GetComponent<MineExplosion>().StartExplosion(explosionRange, true);
        }
        
        if(ExplosionTypeDiagonal){
            
            cellsToDestroy.AddRange(GetCellsDiagonal(explosionRange));
        
            GameObject explosion = Instantiate(FindObjectOfType<GamePrefabs>().explosionPrefab, transform.position, Quaternion.identity);
            explosion.GetComponent<MineExplosion>().StartExplosion(explosionRange, false);
        }
        
        exploded = true;
        
        int destroyCount = 0;
        
        for (int i = 0; i < cellsToDestroy.Count; i++){
            
            if(cellsToDestroy[i] == null) continue;
            
            CellMine cellMine = cellsToDestroy[i].GetComponent<CellMine>();
            
            if(cellMine != null){
                
                if(!cellMine.exploded && cellMine.marked){
                    
                    FindObjectOfType<LevelManager>().AddMineExploded(cellMine);
                }
                else if(!cellMine.marked && !cellsToDestroy[i].destroyedExplosion){
                    
                    destroyCount++;
                    cellsToDestroy[i].DestroyExploded();
                }
            }
            else if(!cellsToDestroy[i].destroyedExplosion){
                
                destroyCount++;
                cellsToDestroy[i].DestroyExploded();
            }
        }
        
        FindObjectOfType<ScoreManager>().AddScore(destroyCount * 10);
        FindObjectOfType<GameTimeLimit>().AddTimer();
        
        AnimationUpdate(animExploded);
        
        Instantiate(FindObjectOfType<GamePrefabs>().matchPrefab, transform.position, Quaternion.identity);
        
        FindObjectOfType<AudioManager>().Play(SoundsNames.Explosion);
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        StartCoroutine(levelManager.BoardUpdateNextFrame());
        
        levelManager.RemoveMineFromQueue(this);
    }
    
    public void ExplosionUpdate(){
        
        if(!exploded) return;
        
        DebugUtility.DrawBox(transform.position, Vector2.one * GameValues.CellSize * .25f, Color.black);
            
        explosionDurationTimer += Time.deltaTime;
        
        if(explosionDurationTimer < explosionDurationLimit) return;
        
        exploded = false;
        
        FindObjectOfType<LevelManager>().OnExplosionEnded();
        
        DestroyExploded();
    }
    
    public void ExplosionTypeRandomize(){
        
        int[] typeChance = new int[]{ 5, 5, 1 };
        
        int chanceSum = 0;
        Array.ForEach(typeChance, value => chanceSum += value);
        
        int typeRandom = Random.Range(0, chanceSum);
        
        int typeChanceSum = 0;
        
        for (int i = 0; i < typeChance.Length; i++){
            
            typeChanceSum += typeChance[i];
            
            if(typeRandom < typeChanceSum){
                
                switch (i){
                    
                    case 0:
                        explosionType = ExplosionTypes.cardinal;
                        break;
                        
                    case 1:
                        explosionType = ExplosionTypes.diagonal;
                        break;
                        
                    case 2:
                        explosionType = ExplosionTypes.adjacent;
                        break;
                }
                break;
            }
        }
    }
}
