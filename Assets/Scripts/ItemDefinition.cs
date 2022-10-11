using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct ItemDefinition
{
    public string ID;
    public string itemName;
    public string description;

    // TODO: current: just rectangle, future: sets of sprites

    public Sprite icon;
    public Dimensions dimensions;
}

[Serializable]
public struct Dimensions
{
    public int width;
    public int height;
}


[Serializable]
public class StoredItem
{
    public ItemDefinition Details;
    public ItemVisual RootVisual;
}