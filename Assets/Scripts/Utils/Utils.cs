using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{

    // Get Available Extension Position by DFS
    // TODO: get argument starting position, current: defaultly (0,0)
    public static List<Position> GetAbleExtPos(List<Position> storedPosList)
    {
        List<Position> visitedPos = new List<Position>();
        Stack<Position> stack = new Stack<Position>();
        List<Position> ableExtPos = new List<Position>();

        stack.Push(new Position(0, 0));

        while (stack.Count > 0)
        {
            Position curPos = stack.Pop();
            if (visitedPos.Contains(curPos))
                continue;
            visitedPos.Add(curPos);

            List<Position> neighbors = curPos.GetNeighbors();
            foreach (Position neighbor in neighbors)
            {
                if (!storedPosList.Contains(neighbor))
                {
                    if (!ableExtPos.Contains(neighbor)) ableExtPos.Add(neighbor);
                }
                else
                {
                    stack.Push(neighbor);
                }
            }
        }
        
        return ableExtPos;
    }
}
