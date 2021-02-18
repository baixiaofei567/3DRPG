using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetManager : MonoBehaviour
{
    //四个按钮的功能，以及music滑动调节音量
    public Button continueButton;
    public Button saveButton;
    public Button loadButton;
    public Button quitButton;
    public GameObject setPanle;
    public Button setButton;

    public Slider sliderMain;
    public Slider sliderLevel;

    bool isOpen = false;

    private void Awake()
    {
        continueButton.onClick.AddListener(() =>
        {
            setPanle.SetActive(false);
            isOpen = false;
        });

        saveButton.onClick.AddListener(() =>
        {
            SaveManager.Instance.SavePlayerData();
            QuestManager.Instance.SaveQuestManager();
        });

        loadButton.onClick.AddListener(() =>
        {
            SaveManager.Instance.LoadPlayerData();
            QuestManager.Instance.LoadQuestManager();
        });

        quitButton.onClick.AddListener(() =>
        {
            SceneController.Instance.TransitionToMain();
        });

        setButton.onClick.AddListener(() =>
        {
            isOpen = !isOpen;
            setPanle.SetActive(isOpen);
        });
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Main")
        {
            setBg1Audio();
        }
        else
        {
            setBg2Audio();
        }
        if(isOpen == true)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void setBg1Audio()
    {
        AudioManager.Instance.BG1.volume = sliderMain.value;
    }

    public void setBg2Audio()
    {
        AudioManager.Instance.BG2.volume = sliderLevel.value;
        AudioManager.Instance.attack01.volume = sliderLevel.value;
        AudioManager.Instance.attack02.volume = sliderLevel.value;
    }
}
