using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public int level = 1;
    public float gameSpeed;
    public bool isRunningStage = true;

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
    
    [SerializeField] private BackgroundMove background_1;
    [SerializeField] private BackgroundMove background_2;
    [SerializeField] private BackgroundMove background_3;
    [SerializeField] private BackgroundMove background_7;
    [SerializeField] private BackgroundMove background_8;

    
    [SerializeField] private BackgroundMove2 background_4;
    [SerializeField] private BackgroundMove2 background_5;
    [SerializeField] private BackgroundMove2 background_6;
    
    [SerializeField] private GameObject replay;
    public void GameOver()
    {
        print("GAME OVER");
        replay.SetActive(true);
        // 플레이어와 배경 멈추기

        //SetHeightBG(0f);
        //SetWidthBG(0f);
        // 엔딩 띄우기
        
    }

    public void ClickedReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetHeightBG(float speed)
    {
        background_1.SetSpeed(speed);
        background_2.SetSpeed(speed);
        background_3.SetSpeed(speed);
        background_7.SetSpeed(speed);
        background_8.SetSpeed(speed);
    }


    private void SetWidthBG(float speed)
    {
        background_4.SetSpeed(speed);
        background_5.SetSpeed(speed);
        background_6.SetSpeed(speed);
    }

    [SerializeField] private CameraMove cameraMove;
    [SerializeField] private Object width_prefab;
    [SerializeField] private Object height_prefab;
    public float time=0, time2=0;
    private GameObject width;
    private GameObject height;
    

    [SerializeField] private GameObject ground;
    public IEnumerator RunningToFalling()
    {
        Debug.Log("Run to Fall");
        
        if(width!=null)
            Destroy(width);
        ground.SetActive(false);

        float t = 0;
        while (t < stageDelayTime)
        {
            SetWidthBG(Mathf.Lerp(level * 10, 0, t / stageDelayTime));
            t += Time.deltaTime;
            yield return null;
        }

        background_1.gameObject.SetActive(true);
        background_2.gameObject.SetActive(true);
        background_3.gameObject.SetActive(true);
        background_7.gameObject.SetActive(true);
        background_8.gameObject.SetActive(true);

        background_4.gameObject.SetActive(false);
        background_5.gameObject.SetActive(false);
        background_6.gameObject.SetActive(false);
        
        background_5.Transite(-1);

        // 세로 맵 시작
        // 가로 맵 멈추기
        SetHeightBG(level * 10);
    }

    public IEnumerator FallingToRunning()
    {
        Debug.Log("Fall to run");
        
        if(height!=null)
            Destroy(height);
        ground.SetActive(true);

        float t = 0;
        while (t < stageDelayTime)
        {
            SetHeightBG(Mathf.Lerp(level * 10, 0, t /stageDelayTime));
            t += Time.deltaTime;
            yield return null;
        }

        background_1.gameObject.SetActive(false);
        background_2.gameObject.SetActive(false);
        background_3.gameObject.SetActive(false);
        background_7.gameObject.SetActive(false);
        background_8.gameObject.SetActive(false);

        background_4.gameObject.SetActive(true);
        background_5.gameObject.SetActive(true);
        background_6.gameObject.SetActive(true);

        background_2.Transite(-1);
        background_3.Transite(-1);

        // 세로 맵 멈추기
        // 가로 맵 시작
        SetWidthBG(level * 10);
    }

    private void transiteBG2()
    {
        background_2.Transite(1);
        background_3.Transite(1);
    } 

    private void Update()
    {
        time += Time.deltaTime;
        time2 += Time.deltaTime;

        if((int)time2%60 >= 14)
        {
            print("Map Change");
            if(isRunningStage)
            {
                background_5.Transite(1);
            }
            else
            {
                Invoke("transiteBG2", 0.5f);
            }
            time2=0f;
        }

        // 1분 30초 경과 시
        if((int)time%60 >= 15)
        {
            print("Timer Over");
            if(isRunningStage)
            {
                width = Instantiate(width_prefab, background_4.transform) as GameObject;
            }
            else
            {
                height = Instantiate(height_prefab, background_2.transform) as GameObject;
            }
            
            time = 0f;
            time2=0f;
        }

        // 뛰는 상황
        if(isRunningStage)
        {

            // 카메라는 캐릭터 따라 다니기
            //cameraMove.cameraOn = true;
        }
        // 떨어지는 상황
        else
        {
            cameraMove.cameraOn = false;
            

        }
    }
    

}
