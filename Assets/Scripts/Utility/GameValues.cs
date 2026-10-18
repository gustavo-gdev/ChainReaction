using UnityEngine;

public static class GameValues{
    
    public static int CellSize{ get{ return 1; } }
    public static int CellsPerChunk{ get{ return 5; } }
    public static Vector2 BoardSize{ get { return Vector2.one * 10; } }
    public static int BoardCellsCount{ get { return (int)(BoardSize.x * BoardSize.y); } }
}
