using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotUI[] slotUIs;

    public void RefreshUI()
    {
        //length就是一个属性，c++就得用size()这个函数
        for(int i = 0; i < slotUIs.Length; ++i)
        {
            slotUIs[i].itemUI.Index = i;
            slotUIs[i].UpdateItem();
        }
    }
}
