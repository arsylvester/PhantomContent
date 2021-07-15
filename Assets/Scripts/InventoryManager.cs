using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static int[] inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new int[InteractableObject.NUM_ITEM_TYPES];
    }

    public static void inventoryUpdate(InteractableObject.ItemTypes item, int quantity)
    {
        inventory[(int)item] += quantity;
    }
}
