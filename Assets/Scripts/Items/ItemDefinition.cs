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

[Serializable]
public struct Position
{
    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;
    public static bool operator ==(Position a, Position b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Position a, Position b)
    {
        return !(a == b);
    }

    public static Position operator +(Position a, Position b)
    {
        return new Position(a.x + b.x, a.y + b.y);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}