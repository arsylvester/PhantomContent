using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public static List<InteractableObject> allInteractable = new List<InteractableObject>();
    public const int NUM_ITEM_TYPES = 5;
    public enum InteractableTypes {NPC, APPLE, FISH, CAR_KEYS, BED, PACKAGE};
    

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
        else if (type == InteractableTypes.APPLE)
            QuestMaster.instance.AppleUpdated();
        else if (type == InteractableTypes.BED)
        {
            MenuManager mm = GameObject.FindObjectOfType<MenuManager>();
            mm.NextDay();
            mm.RunDayEndSequence();
        }
        else if (type == InteractableTypes.CAR_KEYS)
            QuestMaster.instance.FoundKeys();
        else if (type == InteractableTypes.PACKAGE)
            QuestMaster.instance.PickupPackage();
        Debug.Log("Destroying item");
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        allInteractable.Remove(this);
    }
}
