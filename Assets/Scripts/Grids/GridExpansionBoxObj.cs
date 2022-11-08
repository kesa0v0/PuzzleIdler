using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridExpansionBoxObj : MonoBehaviour
{
    public Position pos;
    private bool isAlreadyClicked = false; // To prevent double click

    public void Setup(Position pos)
    {
        this.pos = pos;
        this.transform.localPosition = new Vector3(pos.x, pos.y, 5);
        this.name = "GridExtBox " + pos.x + " " + pos.y;
    }

    private void OnMouseDown() 
    {
        if (!isAlreadyClicked)
        {
            isAlreadyClicked = true;
            Debug.Log("Clicked: " + pos.ToString());
            GridManager.Instance.ExpandGrid(this);
        }
    }
}
