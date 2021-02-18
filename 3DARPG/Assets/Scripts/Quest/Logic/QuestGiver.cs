using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    DialogController controller;

    QuestData_SO currentQuest;

    public DialogueData_SO startDialogue;

    public DialogueData_SO progressDialogue;

    public DialogueData_SO completeDialogue;

    public DialogueData_SO finishDialogue;

    #region 获得任务状态
    public bool IsStarted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsStarted;
            }
            else return false;
        }
    }

    public bool IsComplete
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsCompleted;
            }
            else return false;
        }
    }
    public bool IsFinished
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsFinished;
            }
            else return false;
        }
    }
    #endregion

    private void Awake()
    {
        controller = GetComponent<DialogController>();
    }

    private void Start()
    {
        controller.currentData = startDialogue;
        currentQuest = controller.currentData.GetQuest();
    }

    private void Update()
    {
        //判断当前任务的进度
        if (IsStarted)
        {
            if (IsComplete)
            {
                controller.currentData = completeDialogue;
            }
            else
            {
                controller.currentData = progressDialogue;
            }
        }
        if (IsFinished)
        {
            controller.currentData = finishDialogue;
        }
    }
}
