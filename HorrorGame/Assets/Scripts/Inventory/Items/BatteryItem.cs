using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryItem : Item
{
    void Awake()
    {
        itemName = "Batteries";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void UseItem()
    {
        Debug.Log("Battery level: " + GameManager.instance.batteryLevel);
        Debug.Log("Used battery!");
        GameManager.instance.batteryLevel += 60.0f;
        Debug.Log("Battery level: " + GameManager.instance.batteryLevel);
    }
    public override string ToString()
    {
        return itemName + "item";
    }
}
