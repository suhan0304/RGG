using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    protected string itemCode;
    protected string itemName;
    protected int power;
    public string getItemCode()
    {
        return itemCode;
    }
    public string getItemName()
    {
        return itemName;
    }
    public int getPower()
    {
        return power;
    }
}
