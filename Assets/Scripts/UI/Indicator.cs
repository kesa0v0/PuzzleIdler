using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject indicatorCellPrefab;
    
    public void IndicateOn(ItemObj itemObj)
    {
        this.gameObject.SetActive(true);
        this.transform.position = itemObj.transform.position + new Vector3(0, 0, -1);
        
        // copy shape of item
        // Value -> Key 이거 문제 될 수 도 잇는데 몰?루
        foreach (var cell in itemObj.cellSet.Keys)
        {
            var indicatorCell = Instantiate(indicatorCellPrefab, this.transform);
            indicatorCell.transform.localPosition = new Vector3(cell.x, cell.y, 0);
        }
    }

    public void GridIndicate(ItemObj itemObj)
    {
        // set indicator's position
        this.transform.SetParent(GridManager.Instance.GridParentObj.transform);
        var gridPos = GridManager.Instance.GetGridRelativePosition(itemObj.transform.position);
        this.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);
        this.transform.localScale = Vector3.one;

        // set indicator's color
        var indicatorCells = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (var cell in indicatorCells)
        {
            // check if valid position
            //TODO: cell 별로 확인하기
            if (GridManager.Instance.IsPositionAvailable(itemObj, GridManager.Instance.GetGridRelativePosition(this.transform.position)))
            {
                // set color to green, a = 50%
                cell.color = new Color(0, 1, 0, 0.5f);
            }
            else
            {
                // set color to red, a = 50%
                cell.color = new Color(1, 0, 0, 0.5f);
            }
        }
    }

    public void ItemInventoryIndicate(ItemObj itemObj)
    {
        // Debug.Log("ItemInventoryIndicate");
        var index = Inventory.Instance.GetInsertIndexFromMousePosition();
        
        // Add item to list
        this.transform.SetParent(Inventory.Instance.Content.transform);
        // Set item position
        this.transform.SetSiblingIndex(index);
        this.transform.localScale = Inventory.Instance.inventoryItemScale * 50;


        var indicatorCells = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (var cell in indicatorCells)
        {
            // make it half transparent
            // TODO: load its Original Color
            cell.color = new Color(1, 1, 1, 0.60f);
        }
    }

    public void Indicate(ItemObj itemObj)
    {
        if (Inventory.Instance.IsMouseOnInventory())
        {
            ItemInventoryIndicate(itemObj);
        }
        else
        {
            GridIndicate(itemObj);
        }
    }

    public void IndicateOff()
    {
        this.gameObject.SetActive(false);
        this.transform.SetParent(null);
        this.transform.position = Vector3.zero;
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
