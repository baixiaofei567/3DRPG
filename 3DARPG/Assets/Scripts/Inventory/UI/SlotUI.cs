using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum SlotType { BAG,WEAPON,ARMOR,ACTION}

public class SlotUI : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public ItemUI itemUI;

    public SlotType slotType;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (itemUI.Bag.items[itemUI.Index].itemData != null)
        {
            //先判断该UI对应的数据类型是否是可使用的
            if (itemUI.Bag.items[itemUI.Index].itemData.itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount > 0)
            {
                //用该物品设置好的血量去更新人物对应的血量
                GameManager.Instance.playerStats.ApplyHealth(itemUI.Bag.items[itemUI.Index].itemData.useableItemData.healthPoint);

                itemUI.Bag.items[itemUI.Index].amount -= 1;

                //使用物品也更新任务进度
                QuestManager.Instance.UpdateQuestProgress(itemUI.Bag.items[itemUI.Index].itemData.itemName, -1);
            }
            UpdateItem();
        }
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.WEAPON:
                //切换数据库
                itemUI.Bag = InventoryManager.Instance.equipData;
                //装备武器or切换武器
                if(itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipData;
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];

        itemUI.SetupItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemUI.Bag.items[itemUI.Index].itemData != null)
        {
            InventoryManager.Instance.toolTip.SetUpTooltip(itemUI.Bag.items[itemUI.Index].itemData);
            InventoryManager.Instance.toolTip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.toolTip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        InventoryManager.Instance.toolTip.gameObject.SetActive(false);
    }
}
