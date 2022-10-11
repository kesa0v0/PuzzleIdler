using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVisual : MonoBehaviour
{
    public ItemDefinition itemDefinition;


    public ItemVisual(ItemDefinition itemDefinition)
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
}
