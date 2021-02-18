using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//C#没有多继承，因为怕菱形继承
[System.Serializable]
public class DialogueOption
{
    public string text;

    //要跳到哪一条语句的id
    public string targetID;

    //是否接受任务
    public bool takeQuest;
}
