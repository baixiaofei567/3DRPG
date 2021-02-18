using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRequirement : MonoBehaviour
{
    private Text requeireName;

    private Text progressNumber;

    private void Awake()
    {
        requeireName = GetComponent<Text>();
        progressNumber = transform.GetChild(0).GetComponent<Text>();
    }

    public void SetupRequire(string name, int amount, int currentAmount)
    {
        requeireName.text = name;
        progressNumber.text = currentAmount.ToString() + "/" + amount.ToString();
    }

    public void SetupRequirement(string name, bool isFinished)
    {
        if (isFinished)
        {
            requeireName.text = name;
            progressNumber.text = "完成";
            requeireName.color = Color.gray;
            progressNumber.color = Color.gray;
        }
    }
}
