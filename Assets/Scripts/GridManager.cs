using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
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
        CreateGridVisual();
        LoadGrid();

        GetSample();
    }

    public float cellSize;
    public Vector3 originPosition;
    public GameObject gridVisualPrefab;
    public GameObject[,] gridArray;
    public List<StoredItem> storedItems = new List<StoredItem>();
    public Dimensions gridSize;

    
    public void GetSample()
    {
        // sample item
        var itemVisual = Instantiate(gridVisualPrefab);
        var item = new StoredItem()
        {
            Details = new ItemDefinition()
            {
                ID = "1",
                itemName = "test",
                description = "test",
                icon = Resources.Load<Sprite>("Sprites/Items/Item_1"),
                dimensions = new Dimensions()
                {
                    width = 1,
                    height = 1
                }
            },
            RootVisual = itemVisual.GetComponent<ItemVisual>()
        };
        AddItemToGrid(item.RootVisual, new Vector2Int(0, 0));
    }

    #region UI elements
    // TODO: UI 관련은 따로 분리
    private VisualElement m_Root;
    private static Label m_ItemDetailHeader;
    private static Label m_ItemDetailBody;
    private static Label m_ItemDetailPrice;
    private bool m_IsGridReady;


    private async void Configure()
    {
        m_Root = GetComponentInChildren<UIDocument>().rootVisualElement;
        VisualElement itemDetails = m_Root.Q<VisualElement>("ItemDetails");
        m_ItemDetailHeader = itemDetails.Q<Label>("Header");
        m_ItemDetailBody = itemDetails.Q<Label>("Body");
        m_ItemDetailPrice = itemDetails.Q<Label>("SellPrice");

        await UniTask.WaitForEndOfFrame(this);
        
        m_IsGridReady = true;
    }

    #endregion

    private void CreateGridVisual()
    {
        gridArray = new GameObject[gridSize.width, gridSize.height];

        float currentX = originPosition.x;
        float currentY = originPosition.y;

        for (int x = 0; x < gridSize.width; x++)
        {
            currentY = originPosition.y;

            for (int y = 0; y < gridSize.height; y++)
            {
                GameObject newCell = Instantiate(gridVisualPrefab, new Vector3(currentX, currentY), Quaternion.identity);
                newCell.transform.SetParent(transform);

                gridArray[x, y] = newCell;

                currentY += cellSize;
            }

            currentX += cellSize;
        }
    }

    private async Task<bool> GetPositionForItem(GameObject newItemObj)
    {
        var newItem = newItemObj.GetComponent<StoredItem>();
        for (int y = 0; y < gridSize.height; y++)
        {
            for (int x = 0; x < gridSize.width; x++)
            {
                //try position
                SetItemPosition(newItem, new Vector2(newItem.RootVisual.itemDefinition.dimensions.width * x, 
                    newItem.RootVisual.itemDefinition.dimensions.width * y));
                await UniTask.WaitForEndOfFrame(this);
                // TODO: Make checkOverlaps method
                StoredItem overlappingItem = storedItems.FirstOrDefault(s => 
                    s.RootVisual != null && 
                    checkOverlapping(s.RootVisual, newItem.RootVisual));
                //Nothing is here! Place the item.
                if (overlappingItem == null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool checkOverlapping(ItemVisual rootVisual1, ItemVisual rootVisual2)
    {
        throw new NotImplementedException();
    }

    private static void SetItemPosition(StoredItem element, Vector2 vector)
    {
        element.RootVisual.SetPosition(vector);
    }

    private async void LoadGrid()
    {
        await UniTask.WaitUntil(() => m_IsGridReady);
        foreach (StoredItem loadedItem in storedItems)
        {
            ItemVisual itemVisual = new ItemVisual(loadedItem.Details);

            AddItemToGrid(itemVisual, new Vector2(storedItems.Count % gridSize.width, storedItems.Count / gridSize.width));
            bool gridHasSpace = await GetPositionForItem(itemVisual.gameObject);
            if (!gridHasSpace)
            {
                Debug.Log("No space - Cannot pick up the item");
                RemoveItemFromGrid(itemVisual);
                continue;
            }
            ConfigureItem(loadedItem, itemVisual);
        }
    }
    private void AddItemToGrid(ItemVisual item, Vector2 location)
    {
        item.transform.SetParent(transform);
        item.SetPosition(location);
        item.gameObject.SetActive(true);
    }
    private void RemoveItemFromGrid(ItemVisual item)
    {
        item.transform.SetParent(null);
        item.gameObject.SetActive(false);
    }

    private static void ConfigureItem(StoredItem item, ItemVisual visual)
    {
        item.RootVisual = visual;
        visual.gameObject.SetActive(true);
    }


    // public void AddItemToGrid(ItemDefinition itemDefinition, Vector2Int gridPosition)
    // {
    //     ItemVisual newItemVisual = new ItemVisual(itemDefinition);
    //     newItemVisual.SetPosition(gridArray[gridPosition.x, gridPosition.y].transform.position);

    //     StoredItem newItem = new StoredItem();
    //     newItem.Details = itemDefinition;
    //     newItem.RootVisual = newItemVisual;

    //     storedItems.Add(newItem);
    // }

    // public void RemoveItemFromGrid(ItemDefinition itemDefinition)
    // {
    //     StoredItem itemToRemove = storedItems.Find(x => x.Details.ID == itemDefinition.ID);

    //     if (itemToRemove != null)
    //     {
    //         storedItems.Remove(itemToRemove);
    //     }
    // }
}
