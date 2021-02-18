using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;

    private Button thisButton;

    private DialoguePiece currentPiece;

    private string nextPieceID;

    private bool takeQuest;
    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece,DialogueOption option)
    {
        currentPiece = piece;

        optionText.text = option.text;

        nextPieceID = option.targetID;

        takeQuest = option.takeQuest;
    }

    public void OnOptionClicked()
    {
        //如果这段对话有任务
        if(currentPiece.questData != null)
        {
            //根据当前对话中的任务直接新建并初始化一个任务对象
            var newTask = new QuestManager.QuestTask
            {
                questData = Instantiate(currentPiece.questData)
            };
            if (takeQuest)
            {
                //添加到任务列表
                //判断是否已经有任务了
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //判断是否完成给予奖励
                    if (QuestManager.Instance.GetTask(newTask.questData).IsCompleted)
                    {
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    //没有任务，接收新任务
                    QuestManager.Instance.tasks.Add(newTask);
                    //修改该任务的状态，改newTask没用，因为newTask是临时变量
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;

                    foreach(var require in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(require);
                    }
                }
            }
        }

        //要判断当前选项是否有targetID，也就是后序的piece的序号，如果没有就证明结束了，如果还有就跳到那句话
        if(nextPieceID == "")
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else
        {
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
}
