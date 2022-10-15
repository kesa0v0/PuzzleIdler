using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.EventSystems;

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

    #region UI Control
    [SerializeField] bool isInventoryOpened = true;
    [SerializeField] GameObject inventoryBtn;
    public void ToggleInventory()
    {
        if (isInventoryOpened)
        {
            inventoryBtn.transform.DOLocalMoveX(0, 0.5f);
            this.transform.DOLocalMoveX(-450, 0.5f);
        }
        else
        {
            inventoryBtn.transform.DOLocalMoveX(450, 0.5f);
            this.transform.DOLocalMoveX(0, 0.5f);
        }
        isInventoryOpened = !isInventoryOpened;
    }
    #endregion


    #region Item Control
    public List<ItemObj> storedItems = new List<ItemObj>();
    
    
    public bool IsMouseOnInventory()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void AddItemInventory(ItemObj itemObj)
    {
        storedItems.Add(itemObj);
        itemObj.transform.SetParent(this.transform);
    }

    public void RemoveItemInventory(ItemObj itemObj)
    {
        storedItems.Remove(itemObj);
    }

    #endregion
}
