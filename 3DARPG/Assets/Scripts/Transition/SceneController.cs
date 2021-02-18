using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;

    private GameObject player;
    private NavMeshAgent nav;

    public SceneFade sceneFadePrefab;

    bool fadeFinished = true;
    public void TransitionToDestination(Transtion transtion)
    {
        switch (transtion.transitionType)
        {
            case Transtion.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transtion.destinationTag));
                break;
            case Transtion.TransitionType.DifferentScene:
                StartCoroutine(Transition(transtion.sceneName, transtion.destinationTag));
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);
    }

    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        //TODO:保存数据
        SaveManager.Instance.SavePlayerData();
        InventoryManager.Instance.SaveData();
        QuestManager.Instance.SaveQuestManager();

        //如果不是同场景传送
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetTransitionDestination(destinationTag).transform.position,GetTransitionDestination(destinationTag).transform.rotation);
            //读取数据
            SaveManager.Instance.Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);

            yield break;//跳出协程
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            nav = player.GetComponent<NavMeshAgent>();
            nav.enabled = false;
            player.transform.SetPositionAndRotation(GetTransitionDestination(destinationTag).transform.position, GetTransitionDestination(destinationTag).transform.rotation);
            nav.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetTransitionDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        foreach(var entrance in entrances){
            if(entrance.destinationTag == destinationTag)
            {
                return entrance;
            }
        }

        return null;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
        AudioManager.Instance.BG2Audio();
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    IEnumerator LoadLevel(string scene)
    {
        SceneFade fade = Instantiate(sceneFadePrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));

            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,GameManager.Instance.GetEntrance().rotation);

            //保存数据
            SaveManager.Instance.SavePlayerData();
            InventoryManager.Instance.SaveData();

            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        SceneFade fade = Instantiate(sceneFadePrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = !fadeFinished;
            StartCoroutine(LoadMain());
        }
    }
}
