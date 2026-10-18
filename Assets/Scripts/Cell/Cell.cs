using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using cutil = CalcUtility;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(IInputMouseObject))]
public class Cell : MonoBehaviour, IInputMouseObject{
    
    private Animator _animator;
    
    [Space]
    
    private int _cellId;
    [SerializeField]
    private Transform spriteObj;
    
    [Space]
    
    public Vector2 coordinate;
    private BoardGrid _boardGrid;
    
    [Space]
    
    public bool activated;
    public bool marked;
    public bool destroyedExplosion;
    
    [Space]
    
    public bool hovering;
    
    [Space]
    
    public bool holding;
    public bool holded;
    public virtual bool CanDrag{ get { return false; } }
    public GameObject dragingBorder;
    
    [Space]
    
    public List<GameObject> minesOnSiteCountObjects = new();
    public List<GameObject> minesOnSiteBorderObjects = new();
    
    [Space]
    
    public bool falling;
    
    [Space]
    
    public bool swaping;
    public bool swapingBack;
    
    public Cell cellSwaping;
    
    public Vector2 swapLastPosition;
    public static float swapMoveSpeed = 2f;
    
    [Space]
    
    public Vector2 fallGroundTarget;
    public static float fallMoveSpeed = 11f;
    
    
    
    public CellsTypes CellType{
        get{
            if(GetComponent<CellEmpty>() != null) return CellsTypes.Empty;
            if(GetComponent<CellMine>() != null) return CellsTypes.Mine;
            return CellsTypes.None;
        }
    }
    
    public string animActivate;
    public string animMarked;
    
    public string animIdle;
    public string animHover;
    
    [Space]
    
    [SerializeField]
    private Animator _animatorOverlay;
    
    public string animOverlayInactive;
    public string animOverlayHover;
    public string animOverlayDrag;
    public string animOverlayMiss;
    
    [Space]
    
    public Cell dragOverlayObj;
    
    // ====== ====== ====== ======
    
    
    public virtual void OnMouseClickLeftDown(){
        
        holding = true;
        
        AnimationOverlayUpdate(animOverlayDrag);
    }
    public virtual void OnMouseClickRightDown(){ }
    
    public void OnMouseClickLeftUp(){
        
        holding = false;
        CellDragOverlayEnd();
        
        AnimationOverlayUpdate(animOverlayInactive);
        
        if(!MouseMoved()){
            
            ActivateCell();
        }
        else{
            
            OnDragged();
        }
    }
    
    public void OnMouseClickRightUp(){
        
        if(marked){
            
            Unmark();
        }
        else{   
             
            MarkCell();
        }
    }
    
    public void OnMouseHover(){
        
        if(!CanHover || GameOver.gameOver) return;
        
        AnimationOverlayUpdate(animOverlayHover);
        
        if(!hovering){
            
            hovering = true;
            
            FindObjectOfType<AudioManager>().Play(SoundsNames.Hover);
        }
    }
    
    public void OnMouseHoverOut(){
        
        AnimationOverlayUpdate(animOverlayInactive);
        
        hovering = false;
    }
    
    // ====== ====== ====== ======
    
    public virtual void Awake(){
        
        _animator = GetComponent<Animator>();
        
        _cellId = FindObjectOfType<LevelManager>().cellIdNext;
        FindObjectOfType<LevelManager>().cellIdNext++;
        
        transform.name += $" {_cellId}";
    }
    
    public virtual void Start(){
        
        if(falling){
            
            FallStart();
        }
    }

    public virtual void OnEnable(){ }

    public virtual void OnDisable(){ }
    
    public virtual void ActivateCell(){
        
        if(activated || marked) return;
        
        activated = true;
        marked = false;
        
        FindObjectOfType<AudioManager>().Play(SoundsNames.Empty);
        
        _boardGrid.OnActivatedCell(this);
    }
    
    
    // ====== ====== ====== ======
    
    
    public virtual void Update(){
        
        CellDragOverlayUpdate();
    }
    
    public virtual void LateUpdate(){
        
        CellSwapMovement();
    }
    
    // ====== ====== ====== ======
    
    
    public void OnCreate(Vector2 coordinate, BoardGrid levelGrid){
        
        this.coordinate = coordinate;
        _boardGrid = levelGrid;
    }
    
    public void CoordinateUpdate(Vector2 coordinate){
        
        this.coordinate = coordinate;
    }
    
    public void CoordinateUpdate(){
        
        coordinate = cutil.Vector2ToInt((Vector2)transform.position - FindObjectOfType<BoardGrid>().PositionCornerCell);
    }
    
    
    // ====== ====== ====== ======
    
    
    public virtual void OnDragged(){
        
        Cell cellInCursorDirection = CellInCursorDirection();
        
        if(cellInCursorDirection == null) return;
        
        DragAction();
    }
    
    
    // ====== ====== ====== ======
    
    
    public virtual void MarkCell(){
        
        if(activated) return;
    }
    
    public virtual void Unmark(){ }
    
    
    
    // ====== ====== ====== ======
    
    
    public void DragAction(){
        Cell cellOnCursorDirection = CellInCursorDirection();
        
        if(cellOnCursorDirection == null) return;
            
        CellsSwap(cellOnCursorDirection);
    }
    
    public void CellsSwap(Cell cellToSwap, bool swapBack = false){
        
        if(cellToSwap == null) return;
        
        cellSwaping = cellToSwap;
        
        swaping = true;
        cellSwaping.swaping = true;
        
        swapingBack = swapBack;
        cellSwaping.swapingBack = swapBack;
        
        (transform.position, cellSwaping.transform.position) = (cellSwaping.transform.position, transform.position);
        (coordinate, cellSwaping.coordinate) = (cellSwaping.coordinate, coordinate);
        
        CellSwapMovementStart(cellSwaping.transform.position);
        cellSwaping.CellSwapMovementStart(transform.position);
        
        (coordinate, cellSwaping.coordinate) = (cellSwaping.coordinate, coordinate);
    }
    
    public void CellSwapMovementStart(Vector2 lastPosition){
        
        AnimationOverlayUpdate(animOverlayInactive);
        
        swapLastPosition = lastPosition;
        spriteObj.position = lastPosition;
    }
    
    public void CellSwapMovement(){
        
        if(!swaping) return;
            
        spriteObj.transform.position = Vector2.MoveTowards(spriteObj.transform.position, 
            transform.position, 
            swapMoveSpeed * Time.deltaTime);
        
        if((Vector2)spriteObj.position == (Vector2)transform.position){
            
            if(!swapingBack){
                
                StartCoroutine(MatchCheck(cellSwaping));
            }
            else{
                
                swaping = false;
                swapingBack = false;
                
                if(cellSwaping != null){
                    
                    cellSwaping.swaping = false;
                    cellSwaping.swapingBack = false;
                }
                
                cellSwaping = null;
            }
        }
        
    }
    
    
    // ====== ====== ====== ======
    
    public IEnumerator MatchCheck(Cell cellToSwap){
        
        yield return null;
        
        MatchManager matchManager = FindObjectOfType<MatchManager>();
        
        if(GetComponent<CellMine>() != null){
            
            matchManager.MatchCheck(GetComponent<CellMine>());
        }
        
        if(!FindObjectOfType<LevelManager>().CellExplodingQueue){
            
            CellsSwap(cellToSwap, true);
        }
        else{
                
            swaping = false;
            swapingBack = false;
            
            if(cellSwaping != null){
                
                cellSwaping.swaping = false;
                cellSwaping.swapingBack = false;
            }
            
            cellSwaping = null;
        }
    }
    
    
    // ====== ====== ====== ======
    
    
    public void AnimationUpdate(string animationName){
        
        if(animationName == animHover || animationName == animIdle){
            
            if(!activated && !marked){
                
                _animator.Play(animationName);
            }
        }
        else{
            
            _animator.Play(animationName);
        }
    }
    
    
    public void AnimationOverlayUpdate(string animationName){
        
        if(holding || holded){
            
            if(animationName == animOverlayDrag){
                
                _animatorOverlay.Play(animationName);
            }
            
            return;
        }
        
        if(animationName == animHover || animationName == animIdle){
            
            if(!activated && !marked){
                
                _animatorOverlay.Play(animationName);
            }
        }
        else{
            
            _animatorOverlay.Play(animationName);
        }
    }
    
    
    public void CellDragOverlayUpdate(){
        
        if(!holding) return;
            
        Cell cellInCursorDirection = CellInCursorDirection();
        
        if(dragOverlayObj == cellInCursorDirection) return;
        
        if(dragOverlayObj != null && dragOverlayObj.holded){
            
            dragOverlayObj.holded = false;
            dragOverlayObj.AnimationOverlayUpdate(animOverlayInactive);
        }
        
        dragOverlayObj = cellInCursorDirection;
        
        if(cellInCursorDirection == null) return;
        
        dragOverlayObj.holded = true;
        dragOverlayObj.AnimationOverlayUpdate(animOverlayDrag);
    }
    
    public void CellDragOverlayEnd(){
        
        if(dragOverlayObj == null) return;
        
        dragOverlayObj.holded = false;
        dragOverlayObj.AnimationOverlayUpdate(animOverlayInactive);
        dragOverlayObj = null;
    }
    
    
    // ====== ====== ====== ======
    
    
    public bool MouseMoved(){
        
        bool moved = false;
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        
        if(rayHit.collider){
        
            IInputMouseObject inputObj = rayHit.collider.attachedRigidbody.GetComponent<IInputMouseObject>();
            
            if(inputObj != GetComponent<IInputMouseObject>()){
                
                moved = true;
            }
        }
        else{
            
            moved = true;
        }
        
        return moved;
    }
    
    
    // ====== ====== ====== ======
    
    
    public void DestroyExploded(bool revealCells = true){
        
        DebugUtility.DrawBox(transform.position, Vector2.one * GameValues.CellSize / 2 * .5f, Color.yellow);
        
        BoardGrid levelGrid = FindObjectOfType<BoardGrid>();
        levelGrid.cells.Remove(this);
        
        SetCellsAboveFalling();
        
        StartCoroutine(FindObjectOfType<LevelManager>().BoardUpdateNextFrame());
        
        if(!gameObject.activeSelf) return;
        
        gameObject.SetActive(false);
        
        Destroy(gameObject);
    }
    
    
    // ====== ====== ====== ======
    
    
    public void FallStart(){
        
        falling = true;
        fallGroundTarget = GroundCoordinate();
    }
    
    public void FallMovement(){
        
        Cell cellUnder = GameUtility.CellDirection(transform.position, Vector2.down, 1);
        
        fallGroundTarget = FallGroundTarget();
        
        transform.position = Vector2.MoveTowards(transform.position, fallGroundTarget, fallMoveSpeed * Time.deltaTime);
        
        if((Vector2)transform.position == fallGroundTarget){
            
            falling = false;
            CoordinateUpdate();
        }
        
        Vector2 FallGroundTarget(){
            
            if(cellUnder != null && !cellUnder.falling){
            
                return cellUnder.transform.position + Vector3.up * GameValues.CellSize;
            }
            else if(transform.position.y - GameValues.CellSize <= _boardGrid.PositionCornerCell.y){
                
                return new Vector2(transform.position.x, _boardGrid.PositionCornerCell.y);
            }
            else{
                
                return transform.position + Vector3.down * GameValues.CellSize;
            }
        }
    }
    
    public int CellsFallingUnderCount(){
        
        int gridHeight = _boardGrid.gridSize.y;
        int cellsCount = 0;
        
        for (int i = 0; i < gridHeight; i++){
            
            Cell cell = GameUtility.CellDirection(transform.position, Vector2.down, (i + 1) * GameValues.CellSize);
            
            if(cell != null && cell.falling){
                
                cellsCount++;
            }
        }
        
        return cellsCount;
    }
    
    public Vector2 GroundCoordinate(){
        
        int gridHeight = _boardGrid.gridSize.y;
        int cellsFallingCount = CellsFallingUnderCount();
        int groundCoordinate = 0;
        
        for (int i = 0; i < gridHeight; i++){
            
            Cell cell = GameUtility.CellDirection(transform.position, Vector2.down, (i + 1) * GameValues.CellSize);
            
            if(cell != null && !cell.falling){
                
                groundCoordinate = (int)cell.coordinate.y + 1;
                
                break;
            }
        }
        
        return new Vector2(coordinate.x, groundCoordinate + cellsFallingCount);
    }
    
    public Vector2 CoordinateToPosition(Vector2 coordinate){
        
        return _boardGrid.PositionCornerCell + coordinate * GameValues.CellSize;
    }
    
    
    // ====== ====== ====== ======
    
    
    public bool CanHover{
        get{
            if(swaping) return false;
            return true;
        }
    }
    
    // ====== ======
    
    
    public void SetCellsAboveFalling(){
        
        FindObjectOfType<LevelManager>().fallCheckPositions.Add(transform.position);
    }
    
    
    // ====== ====== ====== ======
    
    
    public Cell CellUnderCursor(){
        
        Cell cellUnderCursor = null;
        
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        
        if(rayHit.collider){
        
            Cell inputObj = rayHit.collider.attachedRigidbody.GetComponent<Cell>();
            
            if(inputObj != null){
                
                cellUnderCursor = inputObj;
            }
        }
        
        return cellUnderCursor;
    }
    
    public Cell CellInCursorDirection(){
        
        if(FindObjectOfType<InputHandler>().CellUnderCursor() == this) return null;
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseCamPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 mouseDirectionRelativeToCell = mouseCamPosition - (Vector2)transform.position;
        Vector2 mouseDirectionAbs = cutil.Vector2Abs(mouseDirectionRelativeToCell);
        Vector2 mouseDirectionAxysMultiplyer = mouseDirectionAbs.x > mouseDirectionAbs.y ? Vector2.right : Vector2.up;
        Vector2 mouseDirectionAxys = (mouseDirectionRelativeToCell * mouseDirectionAxysMultiplyer).normalized;
        
        Vector2 castPosition = (Vector2)transform.position + mouseDirectionAxys * GameValues.CellSize;
        
        RaycastHit2D rayHitCell = Physics2D.Raycast(castPosition, Vector2.right, + GameValues.CellSize * .25f, levelManager.cellLayer);
        
        if(rayHitCell.collider == null) return null;
        
        return rayHitCell.collider.GetComponent<Cell>();
    }
    
    
    // ====== ====== ====== ======
    
    
    public Cell[] GetCellsCardinal(int range = 1){
        
        List<Vector2> directionsCardinal = new(){ Vector2.right, Vector2.up, Vector2.left, Vector2.down };
        
        List<Vector2> directionsCardinalAdd = new();
        
        for (int i = 1; i < range; i++){
            
            directionsCardinalAdd.AddRange(directionsCardinal.Select(value => value * range));
        }
        
        directionsCardinal.AddRange(directionsCardinalAdd);
        
        List<Cell> cells = new();
        
        foreach (Vector2 direction in directionsCardinal){
            
            Cell cell = GameUtility.CellDirection(transform.position, direction, 1);
            
            if(cell != null) cells.Add(cell);
        }
        
        return cells.ToArray();
    }
    
    public Cell[] GetCellsDiagonal(int range = 1){
        
        List<Vector2> directionsDiagonal = new(){ new(1, 1), new(-1, 1), new(-1, -1), new(1, -1) };
        
        List<Vector2> directionsDiagonalAdd = new();
        
        for (int i = 1; i < range; i++){
            
            directionsDiagonalAdd.AddRange(directionsDiagonal.Select(value => value * range));
        }
        
        directionsDiagonal.AddRange(directionsDiagonalAdd);
        
        List<Cell> cells = new();
        
        foreach (Vector2 direction in directionsDiagonal){
            
            Cell cell = GameUtility.CellDirection(transform.position, direction, 1);
            
            if(cell != null) cells.Add(cell);
        }
        
        return cells.ToArray();
    }
}