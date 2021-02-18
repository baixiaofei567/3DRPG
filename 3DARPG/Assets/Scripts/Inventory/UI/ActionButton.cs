using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;

    private SlotUI currentSlotHolder;

    private void Awake()
    {
        currentSlotHolder = GetComponent<SlotUI>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.Bag.items[currentSlotHolder.itemUI.Index].itemData != null)
        {
            currentSlotHolder.UseItem();
        }
    }
}
