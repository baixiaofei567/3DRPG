using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon = null;

    public Text amount = null;

    public ItemData_SO currentItemData;
    //记录该物体属于什么类型的数据库，是inventory还是action
    public Inventory_SO Bag { get; set; }

    public int Index { get; set; } = -1;

    public void SetupItemUI(ItemData_SO item, int itemAmount)
    {
        if(itemAmount == 0)
        {
            Bag.items[Index].itemData = null;
            icon.gameObject.SetActive(false);
            return;
        }
        if(itemAmount < 0)
        {
            item = null;
        }

        if(item != null)
        {
            currentItemData = item;
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }
}
