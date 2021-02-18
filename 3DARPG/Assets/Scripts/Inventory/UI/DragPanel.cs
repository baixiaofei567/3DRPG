using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour,IDragHandler,IPointerDownHandler
{
    RectTransform rectTransform;
    Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //delta是每一帧产生的唯一，anchoredpos是这个Panel的中心点的位置
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //UI层级内下面的先渲染，将当前拖拽的panel显示在它父级的最下方，就可以优先显示
        rectTransform.SetSiblingIndex(2);
    }
}
