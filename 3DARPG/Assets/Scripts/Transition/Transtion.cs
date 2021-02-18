using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transtion : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScene
    }
    [Header("Transtion Info")]

    public string sceneName;

    public TransitionType transitionType;

    public TransitionDestination.DestinationTag destinationTag;

    public bool canTrans;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            //传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
