using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    //定义为public就是为了别的脚本可以通过gm来访问player的属性
    public CharactersStats playerStats;

    private CinemachineFreeLook followCam;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //用观察者模式反向注册的方法，让player生成的时候告诉gm我是playerstats
    public void RigisterPlayer(CharactersStats player)
    {
        playerStats = player;

        followCam = FindObjectOfType<CinemachineFreeLook>();

        if(followCam != null)
        {
            followCam.Follow = playerStats.transform;
            followCam.LookAt = playerStats.transform;
        }
    }

    //当每一个敌人生成的时候加入列表，当死亡的时候移除
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //向所有观察者广播
    public void NotifyObservers()
    {
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if(item.destinationTag == TransitionDestination.DestinationTag.Enter)
            {
                return item.transform;
            }
        }
        return null;
    }
}
