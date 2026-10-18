using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGrid : MonoBehaviour{
    
    private LevelManager _levelManager;
    private int _gridId;
    
    public Vector2Int gridSize;
    public bool regenerateOnClick;
    
    [Space]
    
    public List<Cell> cells = new();
    public GameObject gridOverlay;
    
    void Awake(){
        
        _levelManager = FindObjectOfType<LevelManager>();
        
        _gridId = FindObjectsOfType<BoardGrid>().Length - 1;
        transform.name += $" {_gridId}";
    }
    
    public void SetGrid(Vector2Int gridSize){
        
        this.gridSize = gridSize;
    }
    
    public Vector2Int ChunkArray{
        get{
            return gridSize / GameValues.CellsPerChunk;
        }
    }
    
    public int ChunkCount{
        get{
            return ChunkArray.x * ChunkArray.y;
        }
    }
    
    public CellMine[] Mines(){
        
        List<CellMine> mines = new();
        
        foreach(Cell cell in cells){
            
            if(cell.GetComponent<CellMine>() != null) mines.Add(cell.GetComponent<CellMine>());
        }
        
        return mines.ToArray();
    }
    
    public int MinesCount{
        get{
            return FindObjectsOfType<CellMine>().Length;
        }
    }
    
    public CellEmpty[] EmptyCells(){
        
        List<CellEmpty> empty = new();
        
        foreach(Cell cell in cells){
            
            if(cell.GetComponent<CellEmpty>() != null) empty.Add(cell.GetComponent<CellEmpty>());
        }
        
        return empty.ToArray();
    }
    
    
    public Cell[] CellsRevealed(){
        
        List<Cell> cellsRevealed = new();
        
        foreach (Cell cell in cells){
            
            if(cell.marked || cell.activated){
                
                cellsRevealed.Add(cell);
            }
        }
        
        return cellsRevealed.ToArray();
    }
    
    
    public Cell GetCellByCoordinate(Vector2 coordinate){
        
        for (int i = 0; i < cells.Count; i++){
            
            if(cells[i].coordinate == coordinate) return cells[i];
        }
        
        return null;
    }
    
    
    // ====== ====== ====== ======
    
    
    public void OnActivatedCell(Cell cell){
        
        if(regenerateOnClick){
            
            regenerateOnClick = false;
            
            _levelManager.NewGrid();
            
            FindObjectOfType<BoardGenerator>().GenerateGrid(cell.coordinate);
        }
    }
    
    public void ResetGrid(){
        
        cells.Clear();
    }
    
    public Vector2Int GridChunkSize{
        get{
            return gridSize / GameValues.CellsPerChunk;
        }
    }
    
    public void DestroyThis(){
        
        foreach(Cell cell in cells){
            
            Destroy(cell.gameObject);
        }
        
        Destroy(gameObject);
    }
    
    // ====== ====== ====== ======
    
    public Vector2[] CellsPositions{
        get{
            Vector2[] positions = new Vector2[cells.Count];
            for (int i = 0; i < positions.Length; i++){
                positions[i] = cells[i].transform.position;
            }
            return positions;
        }
    }
    
    public Vector2 PositionCorner{
        get{
            return (Vector2)transform.position - gridSize * GameValues.CellSize / 2;
        }
    }
    
    public Vector2 PositionCornerCell{
        get{
            return PositionCorner + Vector2.one * GameValues.CellSize / 2;
        }
    }
}
