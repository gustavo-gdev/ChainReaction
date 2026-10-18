using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using gutil = GameUtility;

public class BoardGenerator : MonoBehaviour{
    
    private LevelManager _levelManager;
    
    [Space]
    
    public GameObject emptyPrefab;
    public GameObject minePrefab;
    public GameObject gridOverlayPrefab;
    
    public Transform CellsParent{
        get{
            return _levelManager.gridCellParent;
        }
    }
    // ======
    
    public bool useLocalGrid;
    
    // ======
    
    [SerializeField]
    private BoardGrid gridCurrent;
    
    public BoardGrid GridCurrent{
        get{
            if(useLocalGrid)
                return gridCurrent;
            else
                return _levelManager.gridCurrent;
                
        }
    }
    
    public Vector2 GridPositionCenter{
        get{
            if(useLocalGrid)
                return Vector2.zero;
            else
                return _levelManager.gridCurrent.transform.position;
        }
    }
    
    public Vector2 GridPositionCorner{
        get{
            return (Vector2)GridCurrent.transform.position - GridSize * GameValues.CellSize / 2;
        }
    }
    
    public Vector2 GridCornerCell{ // bottom left
        get{
            return GridPositionCorner + Vector2.one * GameValues.CellSize / 2;
        }
    }
    
    public Vector2Int GridSize{
        get{
            if(GridCurrent == null) return Vector2Int.zero;
            return GridCurrent.gridSize;
        }
    }
    
    
    [Space]
    
    public int emptyCellsRangeAtStart;
    
    void Awake(){
        
        _levelManager = FindObjectOfType<LevelManager>();
    }
    
    void Start(){
        
        GenerateGridWithoutMines();
    }
    
    public void GenerateGrid(Vector2 emptyCellsAtStart, bool generateEmptyCell = true, bool minesEachLine = false){
        
        if(GridCurrent == null) return;
        
        List<Vector2> gridCells = new();
        List<Vector2> gridEmpty = new();
        List<Vector2> gridMines = new();
        
        for (int y = 0; y < GridSize.y; y++){
            
            for (int x = 0; x < GridSize.x; x++){
                
                gridCells.Add(new Vector2(x, y));
            }
        }
        
        List<Vector2> emptyCellsAround = new();
        
        if(generateEmptyCell){
        
            for (int x = -emptyCellsRangeAtStart; x <= emptyCellsRangeAtStart; x++){
                
                for (int y = -emptyCellsRangeAtStart; y <= emptyCellsRangeAtStart; y++){
                    
                    if(Mathf.Abs(x) + Mathf.Abs(y) <= emptyCellsRangeAtStart){
                        
                        emptyCellsAround.Add(new Vector2(emptyCellsAtStart.x + x, emptyCellsAtStart.y + y));
                    }
                }
            }
            
            foreach (Vector2 emptyCellAround in emptyCellsAround){
                
                if(gridCells.Contains(emptyCellAround)){
                    
                    gridCells.Remove(emptyCellAround);
                    gridEmpty.Add(emptyCellAround);
                }
            }
        }
        
        if(minesEachLine){
            
            List<int> rangeX = gutil.ShuffleListInt(Enumerable.Range(0, GridSize.x).ToArray()).ToList();
            List<int> rangeY = gutil.ShuffleListInt(Enumerable.Range(0, GridSize.y).ToArray()).ToList();
                        
            int smallerRange = rangeX.Count < rangeY.Count ? rangeX.Count : rangeY.Count;
            
            for (int i = 0; i < smallerRange; i++){
                
                AddMine(new Vector2(rangeX[i], rangeY[i]));
            }
            
            if(rangeX.Count > rangeY.Count){
                
                for (int i = 0; i < rangeX.Count - rangeY.Count; i++){
                    
                    int indexY = Random.Range(0, GridSize.y);
                    Vector2 mine = new(rangeX[rangeY.Count + i], indexY);
                    
                    AddMine(mine);
                }
            }
            else if(rangeY.Count > rangeX.Count){
                
                for (int i = 0; i < rangeY.Count - rangeX.Count; i++){
                    
                    int indexX = Random.Range(0, GridSize.x);
                    Vector2 mine = new(indexX, rangeX[rangeY.Count + i]);
                    
                    AddMine(mine);
                }
            }
        }
        
        int mineCountMax = MinesMaxAvarage - gridMines.Count;
        
        for (int i = 0; i < mineCountMax; i++){
            
            int randomCell = Random.Range(0, gridCells.Count);
            
            AddMine(gridCells[randomCell]);
        }
        
        gridEmpty.AddRange(gridCells);
        BoardGrid levelGrid = GridCurrent.GetComponent<BoardGrid>();
        gridCurrent = levelGrid;
        
        // ====== ====== ====== ======
        
        foreach(Vector2 mine in gridMines){
            
            GameObject mineObj = Instantiate(minePrefab, GridCornerCell + mine * GameValues.CellSize, Quaternion.identity, CellsParent);
            
            Cell cell = mineObj.GetComponent<Cell>();
            
            cell.OnCreate(mine, GridCurrent);
            levelGrid.cells.Add(cell);
        }
        
        foreach(Vector2 emptyCell in gridEmpty){
            
            GameObject emptyCellObj = Instantiate(emptyPrefab, GridCornerCell + emptyCell * GameValues.CellSize, Quaternion.identity, CellsParent);
            
            Cell cell = emptyCellObj.GetComponent<Cell>();
            
            cell.OnCreate(emptyCell, GridCurrent);
            levelGrid.cells.Add(cell);
        }
        
        if(generateEmptyCell){
            
            foreach (Vector2 emptyCellAround in emptyCellsAround){
                
                if(gridEmpty.Contains(emptyCellAround)){
                        
                    GridCurrent.GetCellByCoordinate(emptyCellAround).ActivateCell();
                }
            }
        }
        
        // ====== ====== ====== ======
        
        void AddMine(Vector2 mine){
            
            if(gridCells.Contains(mine)){
                
                gridCells.Remove(mine);
                gridMines.Add(mine);
            }
        }
    }
    
    
    // ====== ====== ====== ======
    
    public void GenerateGridWithoutMines(){
        
        BoardGrid levelGrid = GridCurrent.GetComponent<BoardGrid>();
        
        
        
        for (int y = 0; y < GridSize.y; y++){
            
            for (int x = 0; x < GridSize.x; x++){
                
                Vector2 cellCoordinate = new Vector2(x, y);
                GameObject emptyCellObj = Instantiate(emptyPrefab, GridCornerCell + cellCoordinate * GameValues.CellSize, Quaternion.identity, CellsParent);
            
                Cell cell = emptyCellObj.GetComponent<Cell>();
                
                cell.OnCreate(cellCoordinate, GridCurrent);
                levelGrid.cells.Add(cell);
            }
        }
        
        levelGrid.regenerateOnClick = true;
    }
    
    
    // ====== ====== ====== ======
    
    
    public void GenerateCells(Vector2[] cellsPositionsArray, bool falling = false){
        
        BoardGrid levelGrid = GridCurrent.GetComponent<BoardGrid>();
        
        int minesCount = gridCurrent.MinesCount;
        int minesTargetCount = MinesMaxAvarageRandom;
        int minesToAdd = minesTargetCount - minesCount;
        
        int lowerCoordinate = (int)cellsPositionsArray.Select(value => value.y).ToArray().Min();
        int girdHeight = GridSize.y;
        Vector2 gridTopPosition = Vector2.up * (girdHeight - lowerCoordinate);
        
        List<Vector2> cellsPositions = cellsPositionsArray.ToList();
        
        List<Vector2> cellsMines = new();
        
        for (int i = 0; i < minesToAdd; i++){
            
            int randomIndex = Random.Range(0, cellsPositions.Count);
            
            cellsMines.Add(cellsPositions[randomIndex]);
            cellsPositions.RemoveAt(randomIndex);
        }
        
        foreach(Vector2 cellMine in cellsMines){
            
            Vector2 instantiatePosition = cellMine * GameValues.CellSize + gridTopPosition;
            
            GameObject emptyCellObj = Instantiate(minePrefab, instantiatePosition, Quaternion.identity, CellsParent);
        
            Cell cell = emptyCellObj.GetComponent<Cell>();
            
            cell.OnCreate(PositionToCoordinate(cellMine), GridCurrent);
            cell.falling = falling;
            cell.FallStart();
            
            levelGrid.cells.Add(cell);
        }
        
        foreach(Vector2 cellEmpty in cellsPositions){
            
            Vector2 instantiatePosition = cellEmpty * GameValues.CellSize + gridTopPosition;
            
            GameObject emptyCellObj = Instantiate(emptyPrefab, instantiatePosition, Quaternion.identity, CellsParent);
        
            Cell cell = emptyCellObj.GetComponent<Cell>();
            
            cell.OnCreate(PositionToCoordinate(cellEmpty), GridCurrent);
            cell.falling = falling;
            
            levelGrid.cells.Add(cell);
        }
    }
    
    
    // ====== ====== ====== ======
    
    
    public Vector2 PositionToCoordinate(Vector2 position){
        
        return (gridCurrent.PositionCornerCell + position * new Vector2(-1, 1)) * new Vector2(-1, 1);
    }
    
    
    // ====== ====== ====== ======
    
    public int MinesMaxAvarage{
        get{
            return Mathf.CeilToInt(GridSize.x * GridSize.y * .3f);
        }
    }
    
    public int MinesMaxAvarageRandom{
        get{
            int maxRandom = Mathf.CeilToInt(MinesMaxAvarage * .025f);
            return MinesMaxAvarage + Random.Range(-maxRandom, maxRandom);
        }
    }
}
