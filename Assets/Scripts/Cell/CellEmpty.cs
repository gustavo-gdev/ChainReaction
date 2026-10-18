using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CellEmpty : Cell{
    
    public TextMeshPro valueText;

    public override void Awake(){
        
        base.Awake();
    }
    
    public override void Start(){ }

    public override void OnEnable(){
        
        base.OnEnable();
        
        FindObjectOfType<LevelManager>().OnBoardUpdate += OnBoardUpdate;
    }

    public override void OnDisable(){
        
        base.OnDisable();
        
        if(FindObjectOfType<LevelManager>() != null){
            
            FindObjectOfType<LevelManager>().OnBoardUpdate -= OnBoardUpdate;
        }
    }


    public void OnBoardUpdate(){
        
        UpdateValue();
    }
    
    public void UpdateValue(){
        
        if(valueText == null || !activated){
            
            return;
        }
        
        if(AdjacentMinesCount > 0){
            
            valueText.text = AdjacentMines().Length + "";
            valueText.color = ColorByValues;
        }
        else{
            
            valueText.text = "";
        }
        
        if(AdjacentMinesCount == 0){
            
            ActivateAdjacentCells();
        }
    }
    
    public CellMine[] AdjacentMines(){
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        List<CellMine> mines = new();
        
        for (int x = -1; x <= 1; x++){
            
            for (int y = -1; y <= 1; y++){
                
                Vector2 castPosition = new Vector3(x, y) + transform.position;
                
                if(castPosition == (Vector2)transform.position) continue;
                
                RaycastHit2D rayHitMine = Physics2D.Raycast(castPosition, Vector2.right, + GameValues.CellSize * .25f, levelManager.mineLayer);
                
                if(rayHitMine.collider != null){
                    
                    mines.Add(rayHitMine.collider.GetComponent<CellMine>());
                }
            }
        }
        
        return mines.ToArray();
    }
    
    public int AdjacentMinesCount{
        get{
            return AdjacentMines().Length;
        }
    }
    
    public void ActivateAdjacentCells(){
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        for (int x = -1; x <= 1; x++){
            
            for (int y = -1; y <= 1; y++){
                
                Vector2 castPosition = new Vector3(x, y) + transform.position;
                
                if(castPosition == (Vector2)transform.position) continue;
                
                RaycastHit2D rayHitMine = Physics2D.Raycast(castPosition, Vector2.right, + GameValues.CellSize * .25f, levelManager.emptyLayer);
                
                if(rayHitMine.collider != null){
                    
                    CellEmpty cellEmpty = rayHitMine.collider.GetComponent<CellEmpty>();
                    
                    if(!cellEmpty.activated){
                        
                        cellEmpty.ActivateCell();
                    }
                }
            }
        }
    }
    
    
    
    
    public override void ActivateCell(){
        
        base.ActivateCell();
        
        AnimationUpdate(animActivate);
        
        FindObjectOfType<CellRevealOvertime>().ResetTimer();
        FindObjectOfType<ScoreManager>().AddScore(AdjacentMinesCount);
        
        UpdateValue();
    }

    public override void MarkCell(){
        
        base.MarkCell();
        
        if(activated) return;
        
        FindObjectOfType<AudioManager>().Play(SoundsNames.Miss);
        Instantiate(FindObjectOfType<GamePrefabs>().markFailPrefab, transform.position, Quaternion.identity);
        
        if(!FindObjectOfType<LevelManager>().GameStarted) return;
        
        FindObjectOfType<GameTimeLimit>().DamageTimer();
    }
    
    
    
    public Color ColorByValues{
        get{
            return AdjacentMinesCount switch
            {
                1 => new Color(.62f, .11f, .11f), // Red
                2 => new Color(0.35f, 0.13f, 0.56f), // Purple
                3 => new Color(0.02f, 0.5f, 0.125f), // Green
                4 => new Color(.76f, .13f, .47f), // Magenta
                5 => new Color(.95f, .9f, .05f), // Yellow
                6 => new Color(.21f, .3f, .84f), // Blue
                7 => new Color(0f, 0.46f, 0.56f), // Light Blue
                8 => new Color(.72f, .25f, .03f), // Orange
                _ => Color.black,
            };
        }
    }
}
