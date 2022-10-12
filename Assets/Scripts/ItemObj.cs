using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemObj : MonoBehaviour
{
    public ItemDefinition itemDefinition;
    public List<ItemCellVisual> cells = new List<ItemCellVisual>();

    public void Setup(ItemDefinition itemDefinition)
    {
        this.itemDefinition = itemDefinition;
        name = itemDefinition.ID;
        
        // Generate Stored Cell into Gameobjects
        itemDefinition.dimensions.ForEach(position =>
        {
            var cell = InstantiateItemCell(position);
        });

        // Hide all cells
        gameObject.SetActive(false);
    }

    // Instantiate Cells
    public GameObject InstantiateItemCell(Position relPos)
    {
        var cell = Instantiate(GridManager.Instance.ItemCellObjPrefab, transform);
        cell.name = "Cell " + itemDefinition.ID + "" + relPos.x + " " + relPos.y;

        cell.transform.localPosition = new Vector3(relPos.x, relPos.y, 0);
        cell.GetComponent<ItemCellVisual>().relPosOfItem = relPos;
        
        cells.Add(cell.GetComponent<ItemCellVisual>());

        return cell;
    }

    public void RemoveItemCell(Position relPos)
    {
        var cell = cells.Find(x => x.relPosOfItem.x == relPos.x && x.relPosOfItem.y == relPos.y);
        if (cell == null)
        {
            Debug.LogError("Cell not found");
            return;
        }

        cells.Remove(cell);
        Destroy(cell.gameObject);

        itemDefinition.dimensions.Remove(relPos);
    }

    #region MouseControls
    
    private Vector3 originalPosition;
    private Vector3 rel_Mouse_CenterObj_Pos;
    private bool isDragging = false;

    public void OnMouseDown(ItemCellVisual cell)
    {
        originalPosition = transform.position;
        rel_Mouse_CenterObj_Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        GridManager.Instance.IndicateOn(this);
    
        isDragging = true;
    }

    public void OnMouseUp() {
        // if not Dragging : Do Nothing
        if (!isDragging)
        {
            return;
        }

        GridManager.Instance.IndicateOff();

        // if Dragging : if Drop possible : Drop, if not : Return to original position
        isDragging = false;
        Position gridPosition = GridManager.Instance.GetGridRelativePosition(transform.position);

        Debug.Log("Grid Position : " + gridPosition.x + " " + gridPosition.y);

        if (GridManager.Instance.IsPositionAvailable(this, gridPosition))
        {
            GridManager.Instance.RemoveItemFromGrid(this);
            GridManager.Instance.AddItemToGrid(this, gridPosition);
            return;
        }

        transform.position = originalPosition;
    }

    // Drag this object
    public void OnMouseDrag() {
        // 원래는 드래그 했을 때 오브젝트가 제일 뒤로 날라가는게 버그인데 오히려 더 잘보여서 그냥 냅둠
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -1;
        transform.position = mousePosition - rel_Mouse_CenterObj_Pos;

        GridManager.Instance.Indicate(this);
    }

    #endregion
}
