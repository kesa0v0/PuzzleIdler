using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

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
            },
            point = 5,
        };

        var itemObj = new GameObject("ItemObj").AddComponent<ItemObj>();
        var rect = itemObj.gameObject.AddComponent<RectTransform>();
        itemObj.Setup(itemDef);
        itemObj.gameObject.SetActive(true);

        // make some cell randomly blacked
        itemObj.cells.ForEach(cell =>
        {
            if (UnityEngine.Random.Range(0, 3) == 0)
            {
                cell.SetColor(Color.black);
            }
        });

        // AddItemInventory(itemObj);
        GenerateItemAtInventory(itemObj);
    }

    #region UI Control
    [SerializeField] bool isInventoryOpened = true;
    public void ToggleInventory()
    {
        if (isInventoryOpened)
        {
            // Use DOTween to animate the inventory closing
            transform.DOLocalMoveX(-400 + transform.localPosition.x, 0.5f);
            isInventoryOpened = false;
        }
        else
        {
            // Use DOTween to animate the inventory opening
            transform.DOLocalMoveX(400 + transform.localPosition.x, 0.5f);
            isInventoryOpened = true;
        }
    }
    #endregion


    #region Item Control
    public GameObject Content;

    internal void MoveScroll(float scroll)
    {
        // Debug.Log(scroll);
        var inventoryRect = GameObject.Find("ItemInventory").GetComponent<ScrollRect>();
        // Scroll 
        // Control scroll range
        if (inventoryRect.verticalNormalizedPosition + scroll > 1)
        {
            inventoryRect.verticalNormalizedPosition = 1;
        }
        else if (inventoryRect.verticalNormalizedPosition + scroll < 0)
        {
            inventoryRect.verticalNormalizedPosition = 0;
        }
        else
        {
            inventoryRect.verticalNormalizedPosition += scroll * 1f;
        }
    }

    public List<ItemObj> storedItems = new List<ItemObj>();

    public Vector3 inventoryItemScale;


    public bool IsMouseOnInventory()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(this.GetComponent<RectTransform>(), Input.mousePosition, Camera.main);
    }
    
    public int GetInsertIndexFromMousePosition()
    {
        // get index from mouse position
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var localMousePos = Content.transform.InverseTransformPoint(mousePos);

        // Debug.Log(localMousePos);
        
        // get index from localMousePos
        var index = - (Mathf.RoundToInt(localMousePos.y / Content.GetComponent<VerticalLayoutGroup>().spacing));

        // if index is out of range, return max or min count
        if (index < 0)
        {
            index = 0;
        }
        else if (index > storedItems.Count)
        {
            index = storedItems.Count;
        }


        return index;
    }

    public void AddItemInventory(ItemObj itemObj)
    {
        // Get Index from mouseposition y
        var index = GetInsertIndexFromMousePosition();
        // Debug.Log(index);
        storedItems.Insert(index, itemObj);
        
        // Add item to list
        itemObj.transform.SetParent(Content.transform, false);
        // Set item position
        itemObj.transform.SetSiblingIndex(index);
        itemObj.transform.localScale = inventoryItemScale * 50;
        itemObj.transform.localPosition = new Vector3(itemObj.transform.localPosition.x, 
                                                    itemObj.transform.localPosition.y, 
                                                    -10);   

    }

    public void RemoveItemInventory(ItemObj itemObj)
    {
        if (!storedItems.Contains(itemObj))
        {
            Debug.Log("Item is not in grid");
            return;
        }

        storedItems.Remove(itemObj);
    }

    public void GenerateItemAtInventory(ItemObj itemObj)
    {
        storedItems.Add(itemObj);
        
        // Add item to list at last
        itemObj.transform.SetParent(Content.transform, false);
        itemObj.transform.localScale = inventoryItemScale * 50;
        itemObj.transform.localPosition = new Vector3(itemObj.transform.localPosition.x, 
                                                    itemObj.transform.localPosition.y, 
                                                    -10);  
    }

    public void SortInventory()
    {
        // Sorts
    }


    #endregion
}
