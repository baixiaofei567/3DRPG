using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{   public class DragData
    {
        public SlotUI originalHoder;

        public RectTransform originalParent;
    }
    //TODO:最后添加模板用于保存数据，不用每次都手动调整为初始值
    [Header("Inventory Data")]
    public Inventory_SO inventory_Template;

    public Inventory_SO inventoryData;

    public Inventory_SO action_Template;

    public Inventory_SO actionData;

    public Inventory_SO equipment_Template;

    public Inventory_SO equipData;

    [Header("Container")]
    public ContainerUI inventoryUI;

    public ContainerUI actionUI;

    public ContainerUI equipUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;

    bool isOpen = false;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemToolTip toolTip;


    protected override void Awake()
    {
        base.Awake();
        if (inventory_Template != null) inventoryData = Instantiate(inventory_Template);
        if (action_Template != null) actionData = Instantiate(action_Template);
        if (equipment_Template != null) equipData = Instantiate(equipment_Template);
    }
    private void Start()
    {
        LoadData();
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipUI.RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }
        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth, GameManager.Instance.playerStats.attackData.minDamage, GameManager.Instance.playerStats.attackData.maxDamage);
    }

    public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipData, equipData.name);
    }

    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipData, equipData.name);
    }

    public void UpdateStatsText(int health,int min ,int max)
    {
        healthText.text = health.ToString();
        attackText.text = min + "-" + max;
    }

    #region 检查拖拽物品是否在每一个slot范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        //参数是鼠标位置，循环每一个方格
        for (int i = 0; i < inventoryUI.slotUIs.Length; i++)
        {
            //方格的位置并强制转换
            RectTransform t = inventoryUI.slotUIs[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        //参数是鼠标位置，循环每一个方格
        for (int i = 0; i < actionUI.slotUIs.Length; i++)
        {
            //方格的位置并强制转换
            RectTransform t = actionUI.slotUIs[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInEquipmentUI(Vector3 position)
    {
        //参数是鼠标位置，循环每一个方格
        for (int i = 0; i < equipUI.slotUIs.Length; i++)
        {
            //方格的位置并强制转换
            RectTransform t = equipUI.slotUIs[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region 检测任务物品

    public void CheckQuestItemInBag(string questItemName)
    {
        foreach (var item in inventoryData.items)
        {
            if(item.itemData != null)
            {
                if(item.itemData.itemName == questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
                }
            }
        }
        foreach (var item in actionData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
                }
            }
        }
    }

    #endregion

    //检测背包和快捷栏物品
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }
    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.items.Find(i => i.itemData == questItem);
    }
}
