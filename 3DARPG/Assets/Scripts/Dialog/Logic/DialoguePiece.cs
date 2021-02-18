using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece
{
    public string ID;
    
    public Sprite image;

    [TextArea]
    public string text;

    public QuestData_SO questData;

    public List<DialogueOption> dialogueOptions = new List<DialogueOption>();
}
