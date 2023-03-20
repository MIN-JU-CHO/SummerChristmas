using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Running,
    Falling
};

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public static readonly float durationOfStage = 180;

    private Player player;

    public static int Level { get; private set; } = 1;
    public static int BackgroundPerStage => (Level * 4);
    public static float MoveSpeed => 5f;
    public static GameState CurrentGameState { get; set; } = GameState.Running;
    public static bool IsScrolling = true;
    public float stageDelayTime = 2.0f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void GameOver()
    {
        print("GAME OVER");
        // 플레이어와 배경 멈추기

        //SetHeightBG(0f);
        //SetWidthBG(0f);
        // 엔딩 띄우기
        
    }

    private void Update()
    {
        //
        // time += Time.deltaTime;
        // time2 += Time.deltaTime;
        //
        // if((int)time2%60 >= 14)
        // {
        //     print("Map Change");
        //     if(CurrentGameState is GameState.Running)
        //     {
        //         background_5.Transite(1);
        //     }
        //     else
        //     {
        //         Invoke("transiteBG2", 0.5f);
        //     }
        //     time2=0f;
        // }
        //
        // // 1분 30초 경과 시
        // if((int)time%60 >= 15)
        // {
        //     print("Timer Over");
        //     if(CurrentGameState is GameState.Running)
        //     {
        //         width = Instantiate(width_prefab, background_4.transform) as GameObject;
        //     }
        //     else
        //     {
        //         height = Instantiate(height_prefab, background_2.transform) as GameObject;
        //     }
        //     
        //     time = 0f;
        //     time2=0f;
        // }
        //
        // // 뛰는 상황
        // if(CurrentGameState is GameState.Running)
        // {
        //
        //     // 카메라는 캐릭터 따라 다니기
        //     //cameraMove.cameraOn = true;
        // }
        // // 떨어지는 상황
        // else
        // {
        //     cameraMove.cameraOn = false;
        //     
        //
        // }
    }

    public static async UniTaskVoid RunningToFalling()
    {
        IsScrolling = false;
        instance.player.RunningToFalling();
        await UniTask.WaitUntil(() => instance.player.IsReadyToFalling);
        IsScrolling = true;
        CurrentGameState = GameState.Falling;
        instance.player.SetFallingAttribute();
    }

    public static async UniTaskVoid FallingToRunning()
    {
        IsScrolling = false;
        instance.player.FallingToRunning();
        await UniTask.WaitUntil(() => instance.player.IsGrounded);
        IsScrolling = true;
        CurrentGameState = GameState.Running;
        instance.player.SetRunningAttribute();
    }
}
