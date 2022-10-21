
using System;
using System.Collections.Generic;
using UnityEngine;

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
    public List<Position> GetNeighbors()
    {
        return new List<Position>()
        {
            new Position(0 + this.x, 1 + this.y),
            new Position(1 + this.x, 0 + this.y),
            new Position(0 + this.x, -1 + this.y),
            new Position(-1 + this.x, 0 + this.y)
        };
    }
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

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, 0);
    }

    public override string ToString()
    {
        return base.ToString();
    }

    // log
    public void Log()
    {
        Debug.Log("x: " + x + ", y: " + y);
    }

    // public Position GetGridRelativePosition(Position basePosition)
    // {
    //     return new Position(this.x - basePosition.x, this.y - basePosition.y);
    // }
}