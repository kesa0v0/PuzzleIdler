using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
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
            Configure();
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

    // TODO: Grid도 Gameobj나 StoredGrid로 관리하기. GridObj에 Position 넣기.
    public List<Position> gridPos = new List<Position>();
    public Dictionary<Position, GridObj> grid = new Dictionary<Position, GridObj>();

    public List<ItemObj> storedItems = new List<ItemObj>();

    [Space]

    [SerializeField] GameObject IndicatorObj;

    
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


    #region UI elements
    // TODO: UI 관련은 따로 분리
    private VisualElement ui_Root;


    private static Label ui_ItemDetailHeader;
    private static Label ui_ItemDetailBody;
    private bool IsUIReady;


    private async void Configure()
    {
        ui_Root = GetComponentInChildren<UIDocument>().rootVisualElement;
        VisualElement itemDetails = ui_Root.Q<VisualElement>("ItemDetails");
        ui_ItemDetailHeader = itemDetails.Q<Label>("Header");
        ui_ItemDetailBody = itemDetails.Q<Label>("Body");

        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        
        IsUIReady = true;
    }

    #endregion

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

    public void IndicatorOn(ItemObj itemVisual, Position gridPosition)
    {
        throw new NotImplementedException();
    }
    
    public static void UpdateItemDetails(ItemDefinition item)
    {
        ui_ItemDetailHeader.text = item.itemName;
        ui_ItemDetailBody.text = item.description;
    }

    #endregion

}
