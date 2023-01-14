using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

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
    
    [SerializeField] private Background background;
    public void GameOver()
    {
        print("GAME OVER");
        // 플레이어와 배경 멈추기

        background.SetSpeed(0f);
        // 엔딩 띄우기
        
    }
    
}
