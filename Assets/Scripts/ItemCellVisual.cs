using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCellVisual : MonoBehaviour
{
    public Position relPosOfItem;
    public Position occupyingGridPos;

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }


    #region MouseControls
    private void OnMouseDown() {
        transform.parent.GetComponent<ItemObj>().OnMouseDown(this);
    }
    private void OnMouseUp() {
        transform.parent.GetComponent<ItemObj>().OnMouseUp();
    }
    private void OnMouseDrag() {
        transform.parent.GetComponent<ItemObj>().OnMouseDrag();
    }
    #endregion
}
