using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public const int NUM_ITEM_TYPES = 4;
    public enum InteractableTypes {NPC, APPLE, FISH, CAR_KEYS};

    public static List<InteractableObject> allInteractable = new List<InteractableObject>();
    

    [Header ("Type")]
    public InteractableTypes type;

    private void Start()
    {
        allInteractable.Add(this);
    }


    public void pickUpItem()
    {
        //InventoryManager.inventoryUpdate(type, 1);
        //Debug.Log("Item picked up: " + type + " = " + InventoryManager.inventory[(int)type]);

        if (type == InteractableTypes.FISH)
            QuestMaster.instance.FishUpdated();
        if (type == InteractableTypes.APPLE)
            QuestMaster.instance.AppleUpdated();

        Debug.Log("Destroying item");
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        allInteractable.Remove(this);
    }
}
