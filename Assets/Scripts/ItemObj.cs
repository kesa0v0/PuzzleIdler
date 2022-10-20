using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemObj : MonoBehaviour
{
    public ItemDefinition itemDefinition;
    public Dictionary<Position, ItemCellObj> cellSet = new Dictionary<Position, ItemCellObj>();
    public List<ItemCellObj> storedCellList = new List<ItemCellObj>();

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
        cell.GetComponent<ItemCellObj>().relPosOfItem = relPos;
        
        storedCellList.Add(cell.GetComponent<ItemCellObj>());

        return cell;
    }

    public void RemoveItemCell(Position relPos)
    {
        var cell = storedCellList.Find(x => x.relPosOfItem.x == relPos.x && x.relPosOfItem.y == relPos.y);
        if (cell == null)
        {
            Debug.LogError("Cell not found");
            return;
        }

        storedCellList.Remove(cell);
        Destroy(cell.gameObject);

        itemDefinition.dimensions.Remove(relPos);
    }

    #region MouseControls
    
    private GameObject originalParent;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 rel_Mouse_CenterObj_Pos;
    private bool isDragging = false;

    public void OnMouseDown(ItemCellObj cell)
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        rel_Mouse_CenterObj_Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        originalParent = transform.parent.gameObject;

        GameManager.Instance.indicator.IndicateOn(this);
    
        isDragging = true;
    }

    public void OnMouseUp() {
        // if not Dragging : Do Nothing
        if (!isDragging)
        {
            return;
        }

        GameManager.Instance.indicator.IndicateOff();

        isDragging = false;

        // if Dragging : if Drop possible : Drop, if not : Return to original position

        // Check if Item is on Inventory First

        if (Inventory.Instance.IsMouseOnInventory())
        {
            // if on Inventory : Drop to Inventory
            // Debug.Log("Drop to Inventory");
            Inventory.Instance.RemoveItemInventory(this);
            GridManager.Instance.RemoveItemFromGrid(this);
            Inventory.Instance.AddItemInventory(this);
            return;
        }


        // Check if Item is on Grid

        Position gridPosition = GridManager.Instance.GetGridRelativePosition(transform.position);

        // Debug.Log("Grid Position : " + gridPosition.x + " " + gridPosition.y);

        if (GridManager.Instance.IsPositionAvailable(this, gridPosition))
        {
            // Debug.Log("Drop to Grid");
            Inventory.Instance.RemoveItemInventory(this);
            GridManager.Instance.RemoveItemFromGrid(this);
            GridManager.Instance.AddItemToGrid(this, gridPosition);
            return;
        }

        // Return to original position
        transform.SetParent(originalParent.transform, false);
        // Debug.Log("Return to original position");
        transform.position = originalPosition;
        transform.localScale = originalScale;
    }

    // Drag this object
    public void OnMouseDrag() {
        // 원래는 드래그 했을 때 오브젝트가 제일 뒤로 날라가는게 버그인데 오히려 더 잘보여서 그냥 냅둠
        transform.SetParent(null);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -10;
        rel_Mouse_CenterObj_Pos.z = 0;
        transform.position = mousePosition - rel_Mouse_CenterObj_Pos;
        transform.localScale = Vector3.one;
        
        GameManager.Instance.indicator.Indicate(this);
    }

    #endregion
}
