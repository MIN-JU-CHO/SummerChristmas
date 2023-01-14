using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public int level = 1;
    public float gameSpeed;
    public bool isRunningStage = true;

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
    
    [SerializeField] private BackgroundMove background_1;
    [SerializeField] private BackgroundMove background_2;
    [SerializeField] private BackgroundMove background_3;
    
    //[SerializeField] private BackgroundMove background_W;
    public void GameOver()
    {
        print("GAME OVER");
        // 플레이어와 배경 멈추기

        SetHeightBG(0f);
        //background_W.SetSpeed(0f);
        // 엔딩 띄우기
        
    }

    private void SetHeightBG(float speed)
    {
        background_1.SetSpeed(speed);
        background_2.SetSpeed(speed);
        background_3.SetSpeed(speed);
    }

    [SerializeField] private Player player;
    [SerializeField] private CameraMove cameraMove;
    private void Update()
    {
        // 뛰는 상황
        if(player.IsRunningStage)
        {
            // 세로 맵 멈추기
            SetHeightBG(0f);
            // 카메라는 캐릭터 따라 다니기
            cameraMove.cameraOn = true;
        }
        // 떨어지는 상황
        else
        {
            cameraMove.cameraOn = false;
            // 세로 맵 시작
            SetHeightBG(level * 10);
            //background_W.SetSpeed(level * 10);
        }
    }
    
}
