using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public DialogueData_SO currentData;

    bool canTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            canTalk = false;
        }
    }

    private void Update()
    {
        if(canTalk && Input.GetMouseButtonDown(1))
        {
            OpenDialogue();    
        }
    }

    void OpenDialogue()
    {
        //打开UI面板
        //传输对话信息
        DialogueUI.Instance.UpdateDialogueData(currentData);
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
