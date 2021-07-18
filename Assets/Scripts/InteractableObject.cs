using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public static List<InteractableObject> allInteractable = new List<InteractableObject>();
    public const int NUM_ITEM_TYPES = 5;
    public enum InteractableTypes {NPC, APPLE, FISH, CAR_KEYS, BED};
    

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
        if (type == InteractableTypes.BED)
            GameObject.FindObjectOfType<MenuManager>().NextDay();
        if (type == InteractableTypes.CAR_KEYS)
            QuestMaster.instance.FoundKeys();
        Debug.Log("Destroying item");
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        allInteractable.Remove(this);
    }
}
