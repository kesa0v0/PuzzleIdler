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
        TestGridPosGen();
        CreateGridVisual();
    }


    private void TestGridPosGen()
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

    // gridPos: to save where grids are 근데 밑에껄로 대체가능 할 듯
    public List<Position> gridPos = new List<Position>();
    public Dictionary<Position, GridObj> gridSet = new Dictionary<Position, GridObj>();

    public List<ItemObj> storedItemList = new List<ItemObj>();


    // 그리드 제작. GridCell Instantiate
    private void CreateGridVisual()
    {
        // Create Grids foreach all grid positions
        foreach (var pos in gridPos)
        {
            CreateGridObj(pos);
        }
    }

    public void CreateGridObj(Position pos)
    {
        var gridObj = Instantiate(GridObjPrefab, GridParentObj.transform);
        gridObj.transform.localPosition = new Vector3(pos.x, pos.y, 5);
        gridObj.name = "Grid " + pos.x + " " + pos.y;

        Debug.Log("Created GridObj: " + pos.ToString());

        gridSet.Add(pos, gridObj.GetComponent<GridObj>());
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

            // Check All Cells in Grids
            if (!gridSet.Keys.Contains(cellRelGridPos))
            {
                // Debug.Log("Out of Grid");
                return false;
            }

            // check if wanted grid is already occupied
            if (gridSet[cellRelGridPos].occupiedCell != null && !itemObj.cellSet.ContainsKey(gridSet[cellRelGridPos].occupiedCell.relPosOfItem))
            {
                
                // Debug.Log("Grid is already occupied");
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
        itemObj.transform.localScale = Vector3.one;

        // updates item's cells' relative position of grid
        foreach (var cellKey in itemObj.cellSet.Keys)
        {
            itemObj.cellSet[cellKey].occupyingGridPos = gridPosition + cellKey;
            gridSet[gridPosition + cellKey].occupiedCell = itemObj.cellSet[cellKey];
        }

        storedItemList.Add(itemObj);
    }

    public void RemoveItemFromGrid(ItemObj itemVisual)
    {
        // check if item is in grid
        if (!storedItemList.Contains(itemVisual))
        {
            Debug.Log("Item is not in grid");
            return;
        }

        // remove item from grid
        storedItemList.Remove(itemVisual);
        
        // remove item's cells from grid
        foreach (var cell in itemVisual.cellSet)
        {
            gridSet[cell.Value.occupyingGridPos].occupiedCell = null;

        }

        // move item to outside of grid
        itemVisual.transform.SetParent(null);

    }


    #endregion

    [SerializeField] GameObject GridExtensionBoxPrefab;


    #region  Grid Extension

    private List<GridExtensionBoxObj> gridExtensionBoxObjList = new List<GridExtensionBoxObj>();

    public void VisualizeExtendablePosition()
    {
        var expandablePositions = Utils.GetAbleExtPos(gridPos);

        gridExtensionBoxObjList = new List<GridExtensionBoxObj>();
        

        foreach (var pos in expandablePositions)
        {
            var gridExtBox = Instantiate(GridExtensionBoxPrefab, GridParentObj.transform).GetComponent<GridExtensionBoxObj>();
            gridExtBox.Setup(pos);

            gridExtensionBoxObjList.Add(gridExtBox);
        }
    }

    public void ExtendGrid(Position pos)
    {
        gridPos.Add(pos);
        CreateGridObj(pos);
    }

    public void ExtendGrid(GridExtensionBoxObj gridExtBoxObj)
    {
        gridPos.Add(gridExtBoxObj.pos);
        CreateGridObj(gridExtBoxObj.pos);

        // remove all grid extension box
        foreach (var gridExtBox in gridExtensionBoxObjList)
        {
            Destroy(gridExtBox.gameObject);
        }
    }

    #endregion

}
