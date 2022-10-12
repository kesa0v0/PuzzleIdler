using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemVisual : MonoBehaviour
{
    public ItemDefinition itemDefinition;
    

    public void Setup(ItemDefinition itemDefinition)
    {
        this.itemDefinition = itemDefinition;

        name = itemDefinition.itemName;
        // TODO: current: just rectangle, future: sets of sprites
        transform.localScale = new Vector3(itemDefinition.dimensions.width, itemDefinition.dimensions.height, 1);
        gameObject.SetActive(false);

        this.GetComponent<SpriteRenderer>().sprite = itemDefinition.icon;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }


    #region MouseControls
    
    private Vector3 originalPosition;
    private bool isDragging = false;

    private void OnMouseDown()
    {
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseUp() {
        // if not Dragging : Do Nothing
        if (!isDragging)
        {
            return;
        }

        // if Dragging : if Drop possible : Drop, if not : Return to original position
        // isDragging = false;
        // Vector2Int gridPosition = GridManager.Instance.GetGridPosition(transform.position);
        // if (GridManager.Instance.IsPositionAvailable(gridPosition))
        // {
        //     GridManager.Instance.AddItemToGrid(this, gridPosition);
        //     return;
        // }


        transform.position = originalPosition;
    }

    // Drag this object
    private void OnMouseDrag() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -1;
        transform.position = mousePosition;
    }

    #endregion
}
