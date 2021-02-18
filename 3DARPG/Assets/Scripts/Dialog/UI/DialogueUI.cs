using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Basic Elements")]
    public Image icon;

    public Text mainText;

    public Button nextButton;

    [Header("Options")]
    public GameObject dialoguePanel;

    public RectTransform optionPanel;

    public OptionUI optionPrefab;

    [Header("Data")]

    int currentIndex = 0;
    public DialogueData_SO currentData;


    protected override void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(CountinueDialogue);
    }

    void CountinueDialogue()
    {
        if (currentIndex < currentData.dialoguePieces.Count)
        {
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }
    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentData = data;
        currentIndex = 0;
    }

    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);
        currentIndex++;

        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;
        }
        mainText.text = "";
        //mainText.text = piece.text;
        mainText.DOText(piece.text, 1f);

        //在没有option而且对话piece>1才显示next按钮
        if(piece.dialogueOptions.Count == 0 && currentData.dialoguePieces.Count > 0)
        {
            nextButton.interactable = true;
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            //nextButton.gameObject.SetActive(false);
            //不可点按
            nextButton.interactable = false;
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
        }

        //创建option
        CreateOptions(piece);
    }

    void CreateOptions(DialoguePiece piece)
    {
        if(optionPanel.childCount > 0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < piece.dialogueOptions.Count; i++)
        {
            var option = Instantiate(optionPrefab, optionPanel);
            option.UpdateOption(piece, piece.dialogueOptions[i]);
        }
    }
}
