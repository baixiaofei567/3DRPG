using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    public class QuestTask
    {
        //用传进来的questData来初始化一个新的questData，这样就不会更改原来的任务数据
        public QuestData_SO questData;
        public bool IsStarted { get { return questData.isStarted; } set { questData.isStarted = value; } }
        public bool IsFinished { get { return questData.isFinished; } set { questData.isFinished = value; } }
        public bool IsCompleted { get { return questData.isComplete; } set { questData.isComplete = value; } }

    }

    //这是接收的任务列表，将接收到的任务加入到这个列表中
    public List<QuestTask> tasks = new List<QuestTask>();


    private void Start()
    {
        LoadQuestManager();
    }
    public void LoadQuestManager()
    {
        var questCount = PlayerPrefs.GetInt("QuestCount");
        for(int i = 0; i < questCount; ++i)
        {
            //加载任务时，先获得存储的任务的数量，自己根据保存的任务信息重新创建出对应数量的任务
            var newQuest = ScriptableObject.CreateInstance<QuestData_SO>();
            SaveManager.Instance.Load(newQuest, "task" + i);
            tasks.Add(new QuestTask { questData = newQuest });
        }
    }

    public void SaveQuestManager()
    {
        //将tasks里面的所有任务逐一保存，因为直接保存list不行，list不是一个obj
        PlayerPrefs.SetInt("QuestCount", tasks.Count);
        for(int i = 0; i < tasks.Count; ++i)
        {
            SaveManager.Instance.Save(tasks[i].questData, "task" + i);
        }
    }

    //敌人死亡，拾取物品时调用该函数
    public void UpdateQuestProgress(string requireName, int amount)
    {
        foreach (var task in tasks)
        {
            if (task.IsFinished)
                continue;
            var matchTask = task.questData.questRequires.Find(r => r.name == requireName);
            if(matchTask != null)
            {
                matchTask.currentAmount += amount;
            }
            //每次杀了一个敌人或者捡了一个东西就检测当前任务是否满足需求了
            task.questData.CheckQuestProgress();
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        //可以循环任务列表，然后查找每个任务的名字，看看是否相同
        //这里我们用Linq
        if (data != null)
        {
            //只要有列表中有任意一个任务名字和要接收的任务相同，就返回true
            return tasks.Any(q => q.questData.questName == data.questName);
        }
        else return false;
    }

    //该函数的作用是找到列表中的对应的任务，方便改这个任务的状态
    public QuestTask GetTask(QuestData_SO data)
    {
        return tasks.Find(q => q.questData.questName == data.questName);
    }
}
