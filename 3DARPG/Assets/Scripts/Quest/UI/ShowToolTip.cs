using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemUI currentItemUI;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //鼠标划入先显示tooltip
        QuestUI.Instance.toolTip.gameObject.SetActive(true);
        QuestUI.Instance.toolTip.SetUpTooltip(currentItemUI.currentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
