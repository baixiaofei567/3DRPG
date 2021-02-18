using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory",menuName = "Inventory/Inventory Data")]
public class Inventory_SO : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemData_SO newItemData, int amount)
    {
        bool found = false;
        //如果该物品是可堆叠的，就遍历背包列表，看看是否有相同的物品，如果有，就加上数量即可。
        //如果没找到就遍历背包格子中第一个为空的，将背包的那一位空的设为该物品即可
        if (newItemData.stackable)
        {
            foreach(var item in items)
            {
                if(item.itemData == newItemData)
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }
        for(int i = 0;i < items.Count; ++i)
        {
            if(items[i].itemData == null && !found)
            {
                items[i].itemData = newItemData;
                items[i].amount = amount;
                break;
            }
        }
    }
}

//没有被序列化是看不见的，继承mono自动被序列化
[System.Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;
}
