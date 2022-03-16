using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : Interactable
{
    public bool isLocked;
    public Transform door;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = door.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning(door.name + "has no rigidbody component!");
        }
        if (isLocked)
        {
            rb.isKinematic = true;
        }
        else
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    // Update is called once per frame
    void Update()
    {
           
    }
    public override void Interact()
    {
        if(Inventory.instance.hasItem(typeof(KeyItem)) > 0)
        {
            //UNLOCK DOOR
            if (rb == null)
            {
                Debug.LogWarning(door.name + "has no rigidbody component!");
                return;
            }
            foreach(Item item in Inventory.instance.getItems())
            {
                if(typeof(KeyItem) == item.GetType())
                {
                    Inventory.instance.removeFromInventory(item);
                    break;
                }
            }
            rb.isKinematic = false;
            transform.gameObject.layer = LayerMask.NameToLayer("Default");
            isLocked = false;
        }
        else
        {
            Debug.Log("Player does not have any key!");
        }
    }
    public override string InteractText()
    {
        return "Unlock";
    }

}