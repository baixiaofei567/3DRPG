using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO::将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();

            //装备武器
            //GameManager.Instance.playerStats.EquipWeapon(itemData);

            //检查是否有任务
            //更新人物需求面板
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName,itemData.itemAmount);
            Destroy(gameObject);
        }
    }
}
