using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System;

// 용어정리:
// Grid : 배경
// Item : 전체 퍼즐 조각
// ItemCell, Cell : 퍼즐 조각 한칸
// Indicator : 어디 놓을지 보여주는거

public sealed class GridManager : MonoBehaviour
{
    // singletone
    public static GridManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        TestMethods();
        CreateGridVisual();
    }


    private void TestMethods()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                gridPos.Add(new Position(i, j));
            }
        }

        gridPos.Add(new Position(-1, -1));
        gridPos.Add(new Position(0, -1));
        gridPos.Add(new Position(-1,0));
    }


    [Header("Prefabs")]
    public GameObject ItemCellObjPrefab;
    public GameObject GridObjPrefab;


    [Header("Grid Settings")]
    // gameobj of parents of all grids
    public GameObject GridParentObj;

    // gridPos: for saving
    public List<Position> gridPos = new List<Position>();
    public Dictionary<Position, GridObj> grid = new Dictionary<Position, GridObj>();

    public List<ItemObj> storedItems = new List<ItemObj>();


    
    public void GetSample()
    {
        // sample item
        var itemDef = new ItemDefinition()
        {
            ID = "ItemObj 1",
            itemName = "test",
            description = "test",
            dimensions = new List<Position>()
            {
                new Position(0, 0),
                new Position(0, 1),
                new Position(1, 0),
            }
        };
        
        var itemObj = new GameObject("ItemObj " + storedItems.Count).AddComponent<ItemObj>();
        itemObj.Setup(itemDef);
        itemObj.gameObject.SetActive(true);

        storedItems.Add(itemObj);
    }

    // 그리드 제작. GridCell Instantiate
    private void CreateGridVisual()
    {
        // Create Grids foreach all grid positions
        foreach (var pos in gridPos)
        {
            var gridVisual = Instantiate(GridObjPrefab, GridParentObj.transform);
            gridVisual.transform.localPosition = new Vector3(pos.x, pos.y, 5);
            gridVisual.name = "Grid " + pos.x + " " + pos.y;

            grid.Add(pos, gridVisual.GetComponent<GridObj>());
        }
    }
    

    #region Item Interaction
    public Position GetGridRelativePosition(Vector3 worldPosition)
    {
        var gridPosition = new Position(
            Mathf.RoundToInt(worldPosition.x - GridParentObj.transform.position.x), 
            Mathf.RoundToInt(worldPosition.y - GridParentObj.transform.position.y)
            );
        return gridPosition;
    }

    public bool IsPositionAvailable(ItemObj itemObj, Position gridPosition)
    {
        foreach (var cell in itemObj.itemDefinition.dimensions)
        {
            var cellRelGridPos = new Position(gridPosition.x + cell.x, gridPosition.y + cell.y);
            Debug.Log("Checking Pos of : " + cellRelGridPos.x + " " + cellRelGridPos.y);

            // Check All Cells in Grids
            if (!grid.Keys.Contains(cellRelGridPos))
            {
                Debug.Log("Out of Grid");
                return false;
            }

            // check if wanted grid is already occupied
            if (grid[cellRelGridPos].occupiedCell != null && !itemObj.cells.Any(x => x.occupyingGridPos == cellRelGridPos))
            {
                Debug.Log("Grid is already occupied");
                return false;
            }
            
        }

        return true;
    }

    public void AddItemToGrid(ItemObj itemObj, Position gridPosition)
    {
        // Move Item to Grid
        itemObj.transform.SetParent(GridParentObj.transform);
        itemObj.transform.localPosition = new Vector3(gridPosition.x, gridPosition.y, 0);

        // updates item's cells' relative position of grid
        foreach (var cell in itemObj.cells)
        {
            cell.occupyingGridPos = new Position(gridPosition.x + cell.relPosOfItem.x, gridPosition.y + cell.relPosOfItem.y);
            Debug.Log("Add Cell Pos: " + cell.occupyingGridPos.x + " " + cell.occupyingGridPos.y);
            grid[cell.occupyingGridPos].occupiedCell = cell;
        }

        storedItems.Add(itemObj);
    }

    public void RemoveItemFromGrid(ItemObj itemVisual)
    {
        // check if item is in grid
        if (!storedItems.Contains(itemVisual))
        {
            Debug.Log("Item is not in grid");
            return;
        }

        // remove item from grid
        storedItems.Remove(itemVisual);
        
        // remove item's cells from grid
        foreach (var cell in itemVisual.cells)
        {
            grid[cell.occupyingGridPos].occupiedCell = null;
        }

        // move item to outside of grid
        itemVisual.transform.SetParent(null);

    }

    #region Indicator
    public GameObject indicatorCellPrefab;
    public GameObject IndicatorObj;
    
    public void IndicateOn(ItemObj itemVisual)
    {
        if (IndicatorObj == null)
        {
            IndicatorObj = new GameObject();
        }
        IndicatorObj.SetActive(true);
        
        // copy shape of item
        foreach (var cell in itemVisual.cells)
        {
            var indicatorCell = Instantiate(indicatorCellPrefab, IndicatorObj.transform);
            indicatorCell.transform.localPosition = new Vector3(cell.relPosOfItem.x, cell.relPosOfItem.y, 0);
        }
    }

    public void Indicate(ItemObj itemObj)
    {
        // check if item is in grid else not show indicator
        

        // set indicator's position
        IndicatorObj.transform.SetParent(GridParentObj.transform);
        var gridPos = GetGridRelativePosition(itemObj.transform.position);
        IndicatorObj.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);

        // set indicator's color
        var indicatorCells = IndicatorObj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var cell in indicatorCells)
        {
            // check if valid position
            //TODO: cell 별로 확인하기
            if (IsPositionAvailable(itemObj, GetGridRelativePosition(IndicatorObj.transform.position)))
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

    public void IndicateOff()
    {
        Destroy(IndicatorObj);
        IndicatorObj = new GameObject();
    }
    #endregion

    #endregion

}
