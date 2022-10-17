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

        AddItemInventory(itemObj);
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

    public void AddItemInventory(ItemObj itemObj)
    {
        storedItems.Add(itemObj);
        itemObj.transform.SetParent(Content.transform, false);
        itemObj.transform.localScale = inventoryItemScale * 50;
        itemObj.transform.localPosition = itemObj.transform.localPosition + new Vector3(0, 0, -1);
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

    public void SortInventory()
    {
        // Sorts
    }

    #endregion
}
