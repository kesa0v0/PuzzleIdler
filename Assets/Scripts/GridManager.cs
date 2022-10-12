using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Linq;
using System;

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
        CreateGridVisual();
        UpdateGrid();
    }

    public GameObject Grid;

    public float cellSize;
    public Vector3 originPosition;
    public GameObject gridVisualPrefab;
    public GameObject[,] gridArray;
    public List<StoredItem> storedItems = new List<StoredItem>();
    public Dimensions gridSize;

    [SerializeField] GameObject sampleItemPrefab;

    
    public void GetSample()
    {
        // sample item
        var itemVisual = Instantiate(sampleItemPrefab);
        itemVisual.name = "Sample Item" + storedItems.Count;
        var itemDef = new ItemDefinition()
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
        };
        var item = new StoredItem()
        {
            Details = itemDef,
            RootVisual = itemVisual.GetComponent<ItemVisual>()
        };

        storedItems.Add(item);

        UpdateGrid();
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

        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        
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
                GameObject newCell = Instantiate(gridVisualPrefab, new Vector3(currentX, currentY, 0), Quaternion.identity);
                newCell.transform.SetParent(Grid.transform, false);

                gridArray[x, y] = newCell;

                currentY += cellSize;
            }

            currentX += cellSize;
        }
    }
    
    // TODO: 이거 어차피 나중에 움직일 수 있게 만들면 안쓸꺼임 대충 만들자
    private async Task<bool> GetPositionForItem(StoredItem item)
    {
        for (int y = 0; y < gridSize.height; y++)
        {
            for (int x = 0; x < gridSize.width; x++)
            {
                //try position
                SetItemPosition(item, new Vector2(x, y));
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                StoredItem overlappingItem = storedItems.FirstOrDefault(s => 
                    s.RootVisual != null && 
                    checkOverlapping(item.RootVisual, s.RootVisual));
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
        if (rootVisual1 == null || rootVisual2 == null)
            return false;
        if (rootVisual1 == rootVisual2)
            return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rootVisual1.transform.position, Vector3.forward, Mathf.Infinity);
        var ishit = hits.Any(h => h == rootVisual2.gameObject);
        return hits.Any(h => h.collider.gameObject == rootVisual2.gameObject);
    }

    private static void SetItemPosition(StoredItem element, Vector2 vector)
    {
        element.RootVisual.SetPosition(vector);
    }

    private async void UpdateGrid()
    {
        await UniTask.WaitUntil(() => m_IsGridReady);
        //TODO: 이거 차곡차곡 정리되는거 필요없엉
        foreach (StoredItem loadedItem in storedItems)
        {
            Debug.Log("Loading item: " + loadedItem.RootVisual.name);
            ItemVisual itemVisual = loadedItem.RootVisual;

            // AddItemToGrid(itemVisual, new Vector2(storedItems.Count % gridSize.width, storedItems.Count / gridSize.width));
            bool gridHasSpace = await GetPositionForItem(loadedItem);
            if (!gridHasSpace)
            {
                continue;
            }
            ConfigureItem(loadedItem, itemVisual);
        }
    }
    private void AddItemToGrid(ItemVisual item, Vector2 location)
    {
        item.transform.SetParent(Grid.transform);
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
