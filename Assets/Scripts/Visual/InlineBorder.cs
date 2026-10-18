using UnityEngine;

public class InlineBorder : MonoBehaviour{
    
    private Material _inlineBorderMaterial;
    public SpriteRenderer materialRenderer;
    
    public void OnStartTypeSight(Vector2 cellPivot, Vector2 direction, float scale){
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        Vector2 position = cellPivot + direction * scale / 2 + direction.normalized - direction * .5f * GameValues.CellSize;
        
        transform.position = position + levelManager.gridCurrent.PositionCornerCell;
        transform.localScale = (Vector2.one - CalcUtility.Vector2Abs(direction.normalized) + CalcUtility.Vector2Abs(direction) * scale) * GameValues.CellSize;
        
        
		_inlineBorderMaterial = new Material(materialRenderer.sharedMaterial);
		materialRenderer.material = _inlineBorderMaterial;
        
        _inlineBorderMaterial.SetInt("_UseLocalScale", 0);
        _inlineBorderMaterial.SetVector("_ObjectLocalScale", transform.localScale);
    }
    
    public void OnStartMarked(Vector2 position, Vector2 scale){
        
        transform.position = position;
        transform.localScale = scale * GameValues.CellSize;
        
		_inlineBorderMaterial = new Material(materialRenderer.sharedMaterial);
		materialRenderer.material = _inlineBorderMaterial;
        
        _inlineBorderMaterial.SetInt("_UseLocalScale", 0);
        _inlineBorderMaterial.SetVector("_ObjectLocalScale", transform.localScale);
    }
}
