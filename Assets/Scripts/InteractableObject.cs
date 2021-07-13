using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header ("Type")]
    public bool isNPC;
    public bool isPickUp;

    [Header ("Status")]
    public bool isAvailable = true;

    private void Start()
    {
        isAvailable = true;
    }

    public void pickUpItem()
    {
        Debug.Log("Destroying item");
        isAvailable = false;
        this.gameObject.SetActive(false);
    }
}
