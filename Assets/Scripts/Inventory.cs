using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

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


    [SerializeField] bool isInventoryOpened = true;
    public void ToggleInventory()
    {
        if (isInventoryOpened)
        {
            this.transform.DOMoveX(-450, 0.5f);
        }
        else
        {
            this.transform.DOMoveX(0, 0.5f);
        }
        isInventoryOpened = !isInventoryOpened;
    }

    public 
    public List<ItemObj> storedItems = new List<ItemObj>();

    public void GetInventoryRelativePosition()
    {

    }


    public void AddItemInventory(ItemDefinition itemDefinition)
    {
    }
}
