using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public const int NUM_ITEM_TYPES = 3;
    public enum ItemTypes {NPC, APPLE, FISH};
    

    [Header ("Type")]
    public bool isNPC;
    public bool isPickUp;
    public ItemTypes itemType;

    [Header ("Status")]
    public bool isAvailable = true; //if this is false, the player will not be able to interact with this object

    private void Start()
    {
        isAvailable = true;
    }


    public void pickUpItem()
    {
        Debug.Log("Destroying item");
        isAvailable = false;
        this.gameObject.SetActive(false);

        InventoryManager.inventoryUpdate(itemType, 1);
        Debug.Log("Item picked up: " + itemType + " = " + InventoryManager.inventory[(int)itemType]);
    }
}
