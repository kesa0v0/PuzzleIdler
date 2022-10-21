using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridExtensionBoxObj : MonoBehaviour
{
    public Position pos;
    public GridExtensionBoxObj(Position pos)
    {
        this.pos = pos;
    }

    public void OnClick()
    {
        Debug.Log("Clicked: " + pos.ToString());
        GridManager.Instance.CreateGridObj(pos);
        
    }
}
