using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public AudioSource BG1;
    public AudioSource BG2;
    public AudioSource attack01;
    public AudioSource attack02;

    public void BG1Audio()
    {
        BG2.Stop();
        BG1.Play();
    }

    public void BG2Audio()
    {
        BG1.Stop();
        BG2.Play();
    }
    public void attack01Audio()
    {
        attack01.Play();
    }
    public void attack02Audio()
    {
        attack02.Play();
    }
}
