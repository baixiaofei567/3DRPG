using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();

    //因为根据string类型的ID不可以直接当做list的下标来获取其中的元素，所以来一个字典
    public Dictionary<string, DialoguePiece> dialogueIndex = new Dictionary<string, DialoguePiece>();

    //在Inspector窗口更改数据，这个函数就会执行
    private void OnValidate()
    {
        dialogueIndex.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))
            {
                dialogueIndex.Add(piece.ID, piece);
            }
        }
    }

    public QuestData_SO GetQuest()
    {
        QuestData_SO currentQuest = null;
        foreach (var piece in dialoguePieces)
        {
            if(piece.questData != null)
            {
                currentQuest = piece.questData;
            }
        }
        return currentQuest;
    }
}
