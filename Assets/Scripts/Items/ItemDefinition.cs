using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum ItemLevel
{
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Purple
}

[Serializable]
public struct ItemDefinition
{
    public string ID;
    public string itemName;
    public string description;
    public List<Position> dimensions;

    public float point;
    public ItemLevel itemLevel;

}
