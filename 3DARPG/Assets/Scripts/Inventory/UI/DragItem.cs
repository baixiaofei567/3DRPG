using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ItemUI currentItemUI;

    SlotUI currentHolder;

    SlotUI targetHolder;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotUI>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();
        InventoryManager.Instance.currentDrag.originalHoder = GetComponentInParent<SlotUI>();
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;
        //记录原始数据，eventData里有鼠标位置，进入区域等
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //跟随鼠标位置移动
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //放下物品，交换数据
        //是否指向UI物品
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.CheckInInventoryUI(eventData.position) || InventoryManager.Instance.CheckInActionUI(eventData.position) ||
                InventoryManager.Instance.CheckInEquipmentUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotUI>())
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotUI>();
                }
                else
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotUI>();
                }
                //判断是否目标的holder是我的原holder
                if (targetHolder != InventoryManager.Instance.currentDrag.originalHoder)
                {
                    switch (targetHolder.slotType)
                    {
                        case SlotType.BAG:
                            //无论谁拖拽到背包都无所谓
                            SwapItem();
                            break;
                        case SlotType.WEAPON:
                            //需要是武器类型才能拖
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                                SwapItem();
                            break;
                        case SlotType.ARMOR:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                                SwapItem();
                            break;
                        case SlotType.ACTION:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                                SwapItem();
                            break;
                    }
                    //上面是交换数据，下面这两个函数是切换数据库以及显示正确数量和图片
                    currentHolder.UpdateItem();
                    targetHolder.UpdateItem();
                }
            }
        }
        //将其父级设置回来
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);

        //让其正常显示
        RectTransform t = transform as RectTransform;

        t.offsetMax = -Vector2.one * 0;
        t.offsetMin = Vector2.one * 0;
    }

    public void SwapItem()
    {
        //获取要交换的物品在它的数据库中的列表的第几位
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];

        //判断是否是同种物体
        bool isSameItem = tempItem.itemData == targetItem.itemData;

        //如果是是相同物体，且可堆叠
        if(isSameItem && targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        //如果是不同物体，就交换它俩在自己数据库中的位置
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }
}
