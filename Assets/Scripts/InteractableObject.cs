using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public const int NUM_ITEM_TYPES = 3;
    public enum InteractableTypes {NPC, APPLE, FISH};
    

    [Header ("Type")]
    public InteractableTypes type;

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

        InventoryManager.inventoryUpdate(type, 1);
        Debug.Log("Item picked up: " + type + " = " + InventoryManager.inventory[(int)type]);
    }
}
