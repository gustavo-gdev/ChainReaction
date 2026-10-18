using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using gutil = GameUtility;

public class LevelManager : MonoBehaviour{
    
    [SerializeField]
    private GameDifficulty _difficulty;
    public GameDifficulty Difficulty{
        get{
            return _difficulty;
        }
        set{
            FindObjectOfType<GameTimeLimit>().SetTimeLimitMax(value);
            FindObjectOfType<HudManager>().UpdateTimeBar();
            _difficulty = value;
        }
    }
    
    [SerializeField]
    private GameDifficulty _difficultyOnReset;
    public GameDifficulty DifficultyOnReset{
        get{
            return _difficultyOnReset;
        }
        set{
            if(value != Difficulty && GameStarted){
                FindObjectOfType<HudManager>().DifficultyChangeWarning(true);
            }
            _difficultyOnReset = value;
        }
    }
    
    [Space]
    
    public int cellIdNext;
    
    [Space]
    
    public BoardGrid gridCurrent;
    private BoardGenerator _boardGenerator;
    public Transform gridCenterPosition;
    public Transform gridCellParent;
    
    [Space]
    
    public GameObject minesOnSightCountPreafab;
    public GameObject linePreafab;
    public GameObject minesOnSightBorderPreafab;
    
    [Space]
    
    public LayerMask cellLayer;
    public LayerMask mineLayer;
    public LayerMask emptyLayer;
    
    
    [Space]
    
    public int cellsFallingCount;
    public bool CellFalling{
        get{
            return gridCurrent.cells.Where(value => value.falling).ToArray().Length > 0;
        }
    }
    
    [Space]
    
    public List<CellMine> minesExplodedQueue = new();
    
    public bool CellExplodingQueue{
        get{
            return gridCurrent.Mines().Where(value => value.exploded).ToArray().Length + 
                   gridCurrent.Mines().Where(value => value.explodedQueue).ToArray().Length > 0;
        }
    }
    
    public bool CellExploding{
        get{
            return gridCurrent.Mines().Where(value => value.exploded).ToArray().Length > 0;
        }
    }
    
    [Space]
    
    public List<Vector2> fallCheckPositions = new();
		
    public event Action OnBoardUpdate;
    
    public Vector2Int GridSize{
        get{
            return Vector2Int.one * 2;
        }
    }
    
    void Awake(){
        
        _boardGenerator = FindObjectOfType<BoardGenerator>();
        
        DifficultyOnReset = GameDifficulty.None;
    }
    
    void Start(){
        
        NewGrid();
    }
    
    void Update(){
        
        CellsFallControl();
    }
    
    void LateUpdate(){
        
        SetCellsFalling();
    }
    
    
    // ====== ====== ====== ======
    

    public void NewGrid(){
        
        if(gridCurrent != null){
            
            gridCurrent.DestroyThis();
        }
        
        GameObject gridCurrentObj = new();
        
        gridCurrentObj.transform.position = gridCenterPosition.position;
        gridCurrentObj.transform.parent = gridCenterPosition;
        gridCurrentObj.transform.name = "Grid";
        
        Vector2Int gridSize = GridSize * GameValues.CellsPerChunk;
        
        gridCurrent = gridCurrentObj.AddComponent<BoardGrid>();
        gridCurrent.SetGrid(gridSize);
        
        // UpdateCameraSize();
    }
    
    public void NewGridEmpty(){
        
        if(gridCurrent != null){
            
            gridCurrent.DestroyThis();
        }
        
        GameObject gridCurrentObj = new();
        
        gridCurrentObj.transform.position = gridCenterPosition.position;
        gridCurrentObj.transform.parent = gridCenterPosition;
        gridCurrentObj.transform.name = "Grid";
        
        Vector2Int gridSize = GridSize * GameValues.CellsPerChunk;
        
        gridCurrent = gridCurrentObj.AddComponent<BoardGrid>();
        gridCurrent.SetGrid(gridSize);
        
        _boardGenerator.GenerateGridWithoutMines();
    }
    
    
    // ====== ====== ====== ======
    
    
    public void BoardUpdate(){
        
        OnBoardUpdate?.Invoke();
    }
    
    public IEnumerator BoardUpdateNextFrame(){
        
        yield return null;
        
        BoardUpdate();
    }
    
    // ====== ====== ====== ======
    
    public void SetCellsFalling(bool generateCells = true){
        
        if(fallCheckPositions.Count == 0 || !CanSetCellsFalling) return;
        
        StartCoroutine(BoardUpdateNextFrame());
        
        if(generateCells){
            
            _boardGenerator.GenerateCells(fallCheckPositions.ToArray(), true);
        }
        
        List<Cell> cellsToFall = new();
        
        foreach (Vector2 fallCheckPosition in fallCheckPositions){
            
            int gridHeight = gridCurrent.gridSize.y;
            
            for (int i = 0; i < gridHeight; i++){
                
                Cell cell = gutil.CellDirection(fallCheckPosition, Vector2.up, (i + 1) * GameValues.CellSize);
                
                if(cell != null && !cell.falling){
                    
                    cell.falling = true;
                    cellsToFall.Add(cell);
                }
            }
        }
        
        fallCheckPositions.Clear();
        
        foreach(Cell cellToFall in cellsToFall){
            
            cellToFall.FallStart();
        }
    }
    
    public void CheckAllCellsFalling(){
        
        List<Vector2> positions = new();
        
        foreach (Cell cell in gridCurrent.cells){
            
            if(!fallCheckPositions.Contains(cell.transform.position) && !gridCurrent.cells.Contains(cell)){
                
                positions.Add(cell.transform.position);
            }
        }
        
        fallCheckPositions.AddRange(positions);
    }
    
    public bool CanSetCellsFalling{
        get{
            return !CellExplodingQueue;
        }
    }
    
    
    // ======= ======= ======= =======
    
    
    public void OnExplosionEnded(){
        
        if(CellExploding) return;
        
        if(minesExplodedQueue.Count <= 0){
            
            StartCoroutine(BoardUpdateNextFrame());
            return;
        }
        
        CellMine[] minesExplode = minesExplodedQueue.ToArray();
        
        for (int i = 0; i < minesExplode.Length; i++){
            
            CellMine mine = minesExplode[i];
            
            if(mine == null) continue;
            
            mine.explodedQueue = false;
            mine.Explosion();
            
            minesExplodedQueue.Remove(mine);
        }
    }
    
    public void AddMineExploded(CellMine mine){
        
        if(!minesExplodedQueue.Contains(mine)){
            
            mine.explodedQueue = true;
            minesExplodedQueue.Add(mine);
        }
    }
    
    public void RemoveMineFromQueue(CellMine mine){
        
        if(minesExplodedQueue.Contains(mine)){
            
            minesExplodedQueue.Remove(mine);
        }
    }
    
    
    // ======= ======= ======= =======
    
    
    public bool Swaping{
        get{
            return gridCurrent.cells.Where(value => value.swaping).ToArray().Length > 0;
        }
    }
    
    
    
    // ======= ======= ======= =======
    
    
    public void ResetGame(){
        
        GameTimeLimit gameTimeLimit = FindObjectOfType<GameTimeLimit>();
        
        Difficulty = DifficultyOnReset;
        FindObjectOfType<HudManager>().DifficultyChangeWarning(false);
        gameTimeLimit.SetTimeLimitMax(Difficulty);
        
        NewGridEmpty();
        gameTimeLimit.TimerReset();
        FindObjectOfType<GameOver>().GameOverEnd();
        
        FindObjectOfType<ScoreManager>().SetScore();
        FindObjectOfType<ScoreManager>().ReloadHiScoreFromSaved();
    }
    
    
    // ======= ======= ======= =======
    
    
    public void CellsFallControl(){
        
        Cell[] cellsFalling = gridCurrent.cells.Where(value => value.falling).ToArray();
        
        if(cellsFalling.Length == 0 && cellsFallingCount > 0){
                
            BoardUpdate();
        }
        
        if(cellsFallingCount != cellsFalling.Length && cellsFalling.Length == 0){
            
            CheckAllCellsFalling();
        }
        
        cellsFallingCount = cellsFalling.Length;
        
        if(cellsFalling.Length == 0) return;
        
        cellsFalling = SortCellsFromLowerToHigher(cellsFalling);
        
        for (int i = 0; i < cellsFalling.Length; i++){
            
            cellsFalling[i].FallMovement();
        }
    }
    
    public Cell[] SortCellsFromLowerToHigher(Cell[] cells){
        
        return cells.OrderBy(value => value.transform.position.y).ToArray();
    }
    
    public bool GameStarted{
        get{
            if(gridCurrent == null) return false;
            return !gridCurrent.regenerateOnClick;
        }
    }
}
