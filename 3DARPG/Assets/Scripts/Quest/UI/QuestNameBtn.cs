using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameBtn : MonoBehaviour
{
    public Text questNameText;

    public QuestData_SO currentData;

    public Text questContentText;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    //点击事件，最重要，都在这里调用
    void UpdateQuestContent()
    {
        questContentText.text = currentData.description;
        QuestUI.Instance.SetupRequireList(currentData);

        foreach (Transform item in QuestUI.Instance.rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in currentData.rewards)
        {
            QuestUI.Instance.SetupRewardItem(item.itemData,item.amount);
        }
    }

    //在button下显示名字的函数，在questUI中调用
    public void SetupNameButton(QuestData_SO questData)
    {
        currentData = questData;

        //如果任务已完成
        if (questData.isComplete)
        {
            questNameText.text = questData.questName + "(完成)";
        }
        else
        {
            questNameText.text = questData.questName;
        }
    }

}
