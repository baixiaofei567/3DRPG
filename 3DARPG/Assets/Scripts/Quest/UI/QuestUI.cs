using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questPanel;

    public ItemToolTip toolTip;

    bool isOpen;

    [Header("Quest Name")]
    public RectTransform questListTransform;

    public QuestNameBtn questNameBtn;

    [Header("Text Content")]
    public Text questContenText;

    [Header("Requirement")]
    public RectTransform requireTransform;

    public QuestRequirement requirement;

    [Header("Reward Panel")]
    public RectTransform rewardTransform;

    public ItemUI rewardUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);
            questContenText.text = "";

            //显示面板内容
            SetupQuestList();
            if (!isOpen) toolTip.gameObject.SetActive(false);
        }
    }

    public void SetupQuestList()
    {
        //每次打开先将原来的btn,reward,require的列表清空，再生成最新的
        foreach (Transform quest in questListTransform)
        {
            Destroy(quest.gameObject);
        }
        foreach (Transform  item in rewardTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        //生成任务在指定列表下
        foreach (var task in QuestManager.Instance.tasks)
        {
            //生成按钮
            var newTask = Instantiate(questNameBtn, questListTransform);
            //调用按钮的函数，显示任务的正确名字
            newTask.SetupNameButton(task.questData);
            newTask.questContentText = questContenText;
        }
    }

    //因为require以及reward都是在此脚本拿到的，所以在此创建脚本然后在按钮事件中调用
    public void SetupRequireList(QuestData_SO questData)
    {
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (var require in questData.questRequires)
        {
            var q = Instantiate(requirement, requireTransform);
            if (questData.isFinished)
                q.SetupRequirement(require.name, questData.isFinished);
            else
                q.SetupRequire(require.name, require.requireAmount, require.currentAmount);
        }
    }

    public void SetupRewardItem(ItemData_SO itemData, int amount)
    {
        var item = Instantiate(rewardUI, rewardTransform);
        item.SetupItemUI(itemData, amount);
    }
}
