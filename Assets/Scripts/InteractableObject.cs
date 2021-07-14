using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum ItemTypes {NPC, APPLE, FISH};
    public static int[] inventory = new int[3];

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

        inventory[(int)itemType]++;
        //Debug.Log("Apple = " + inventory[(int)itemType]);
    }

    public static void removeFromInvetory(ItemTypes removeType, int quantity)
    {
        inventory[(int)removeType] -= quantity;
    }
}
