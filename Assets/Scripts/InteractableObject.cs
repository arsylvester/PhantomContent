using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public const int NUM_ITEM_TYPES = 4;
    public enum InteractableTypes {NPC, APPLE, FISH, CAR_KEYS};
    

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

        if (type == InteractableTypes.FISH)
            QuestMaster.instance.FishUpdated();
        if (type == InteractableTypes.APPLE)
            QuestMaster.instance.AppleUpdated();
        //InventoryManager.inventoryUpdate(type, 1);
        //Debug.Log("Item picked up: " + type + " = " + InventoryManager.inventory[(int)type]);
    }
}
