using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject indicatorCellPrefab;
    
    public void IndicateOn(ItemObj itemVisual)
    {
        this.gameObject.SetActive(true);
        
        // copy shape of item
        foreach (var cell in itemVisual.cells)
        {
            var indicatorCell = Instantiate(indicatorCellPrefab, this.transform);
            indicatorCell.transform.localPosition = new Vector3(cell.relPosOfItem.x, cell.relPosOfItem.y, 0);
        }
    }

    public void GridIndicate(ItemObj itemObj)
    {
        // set indicator's position
        this.transform.SetParent(GridManager.Instance.GridParentObj.transform);
        var gridPos = GridManager.Instance.GetGridRelativePosition(itemObj.transform.position);
        this.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);

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
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
